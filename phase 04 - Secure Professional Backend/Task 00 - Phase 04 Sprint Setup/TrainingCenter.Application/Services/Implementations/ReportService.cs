using TrainingCenter.Application.DTOs.Report.Responses;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Application.Helpers;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ReportDashboardResponse>> GetDashboardSummaryAsync()
        {
            try
            {
                var totalStudents = await _unitOfWork.Students.CountAsync(s => s.IsActive);
                var activeEnrollments = await _unitOfWork.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);
                var completedEnrollments = await _unitOfWork.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Completed);
                var totalTracks = await _unitOfWork.Tracks.CountAsync(t => !t.IsDeleted);

                var collected = (await _unitOfWork.Payments
                                .GetAllAsync(p => p.PaymentStatus == PaymentStatus.Paid
                                         || p.PaymentStatus == PaymentStatus.PartiallyPaid))
                                .Sum(p => p.Amount);

                var refunded = (await _unitOfWork.Payments
                    .GetAllAsync(p => p.PaymentStatus == PaymentStatus.Refunded))
                    .Sum(p => p.Amount);

                var totalRevenue = collected - refunded;

                var unpaidAmount =(await _unitOfWork.Enrollments
                                .GetAsync(e => e.Status == EnrollmentStatus.Draft,e => new
                                {
                                    e.TrainingTrack!.Price,
                                    Paid = e.Payments
                                        .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                                 || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                        .Sum(p => p.Amount)
                                })).Select(x => x.Price - x.Paid)
                                .Where(owed => owed > 0)   // clamp at enrollment level, not grand total
                                .Sum();

                var response = new ReportDashboardResponse
                {
                    TotalStudents = totalStudents,
                    ActiveEnrollments = activeEnrollments,
                    CompletedEnrollments = completedEnrollments,
                    TotalTracks = totalTracks,
                    TotalRevenue = totalRevenue,
                    UnpaidAmount = unpaidAmount > 0 ? unpaidAmount : 0
                };

                return Result<ReportDashboardResponse>.SuccessResult(response, "Dashboard summary retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<ReportDashboardResponse>.FailureResult("Error retrieving dashboard summary.", ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportUnpaidEnrollmentResponse>>> GetUnpaidEnrollmentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.FailureResult("Validation errors.", errors);


                var unpaidEnrollments = (await _unitOfWork.Enrollments
                            .GetAsync(e => e.Status == EnrollmentStatus.Draft,e => new ReportUnpaidEnrollmentResponse
                            {
                                EnrollmentId = e.Id,
                                StudentTitle = e.Student.FullName ?? string.Empty,
                                TrackTitle = e.TrainingTrack.Title ?? string.Empty,
                                TrackPrice = e.TrainingTrack.Price ,
                                TotalPaid = PaymentCalculator.Calculate(e).TotalPaid,
                                EnrollmentDate = e.EnrollmentDate
                            }))
                            .Where(x => x.TotalPaid < x.TrackPrice)
                            .OrderByDescending(x => x.EnrollmentDate).ToList();

                if (!unpaidEnrollments.Any()) return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.FailureResult("No unpaid enrollments are found.", ".NotFound");

                var totalCount = unpaidEnrollments.Count();
                var items = unpaidEnrollments
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.SuccessResult(new PagedResult<ReportUnpaidEnrollmentResponse>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Unpaid enrollments retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.FailureResult( "Error retrieving unpaid enrollments.", ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportTrackCapacityResponse>>> GetTrackCapacityAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<ReportTrackCapacityResponse>>.FailureResult("Validation errors.", errors);

                var capacityReports = (await _unitOfWork.Tracks
                    .GetAsync(t => !t.IsDeleted, includes:"Enrollments",selector: t => new ReportTrackCapacityResponse
                    {
                        TrackId = t.Id,
                        TrackTitle = t.Title,
                        Capacity = t.Capacity,
                        Enrolled = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed)
                    }))
                    .ToList();

                if (!capacityReports.Any()) return Result<PagedResult<ReportTrackCapacityResponse>>.FailureResult("No Tracks are found.", ".NotFound");

                var totalCount = capacityReports.Count;
                var items = capacityReports
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Result<PagedResult<ReportTrackCapacityResponse>>.SuccessResult(new PagedResult<ReportTrackCapacityResponse>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Track capacity report retrieved successfully.");
   
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ReportTrackCapacityResponse>>.FailureResult("Error retrieving track capacity report.", ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportTrackAvailableSeatsResponse>>> GetTracksWithAvailableSeatsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.FailureResult("Validation errors.", errors);

                var tracks = await _unitOfWork.Tracks
                    .GetAllAsync(t => !t.IsDeleted, "Enrollments");

                var availableSeats = tracks
                    .Select(t =>
                    {
                        var seats = t.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed) ?? 0;
                        var remaining = t.Capacity - seats;
                        return new ReportTrackAvailableSeatsResponse
                        {
                            TrackId = t.Id,
                            TrackTitle = t.Title,
                            Capacity = t.Capacity,
                            ActiveEnrollments = seats
                        };
                    })
                    .Where(r => r.RemainingSeats > 0)
                    .ToList();

                if (!availableSeats.Any()) return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.FailureResult("No Tracks with available are found.", ".NotFound");


                var totalCount = availableSeats.Count;
                var items = availableSeats
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.SuccessResult(new PagedResult<ReportTrackAvailableSeatsResponse>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Tracks with available seats retrieved successfully.");
               
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.FailureResult("Error retrieving tracks with available seats.", ex.Message);
            }
        }

        public async Task<Result<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync()
        {
            try
            {
                var collected = (await _unitOfWork.Payments
                    .GetAllAsync(p => p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.PartiallyPaid))
                    .Sum(p => p.Amount);

                var refunded = (await _unitOfWork.Payments
                    .GetAllAsync(p => p.PaymentStatus == PaymentStatus.Refunded))
                    .Sum(p => p.Amount);

                var paidAmount = (await _unitOfWork.Payments
                    .GetAllAsync(p => p.PaymentStatus == PaymentStatus.Paid))
                    .Sum(p => p.Amount);

                var paidCount = await _unitOfWork.Payments
                    .CountAsync(p => p.PaymentStatus == PaymentStatus.Paid);

                var totalCount = await _unitOfWork.Payments.CountAsync();

                var totalRevenue = collected - refunded;

                var partiallyPaidAmount = (await _unitOfWork.Payments.GetAllAsync(p => p.PaymentStatus == PaymentStatus.PartiallyPaid)).Sum(p => p.Amount);
                var pendingAmount =( await _unitOfWork.Enrollments
                                .GetAsync(e => e.Status == EnrollmentStatus.Draft,e => new
                                {
                                    e.TrainingTrack!.Price,
                                    Paid = e.Payments
                                        .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                                 || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                        .Sum(p => p.Amount)
                                }))
                                .Select(x => x.Price - x.Paid)
                                .Where(owed => owed > 0)   // clamp at enrollment level, not grand total
                                .Sum();

                var response = new ReportRevenueSummaryResponse
                {
                    TotalRevenue = totalRevenue,
                    PaidAmount = paidAmount,
                    PartiallyPaidAmount = partiallyPaidAmount,
                    PendingAmount = pendingAmount,
                    TotalPayments = totalCount,
                    PaidCount = paidCount
                };

                return Result<ReportRevenueSummaryResponse>.SuccessResult(response, "Revenue summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<ReportRevenueSummaryResponse>.FailureResult("Error retrieving revenue summary.", ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportRevenueByTrackResponse>>> GetRevenueByTrackAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<ReportRevenueByTrackResponse>>.FailureResult("Validation errors.", errors);

                var tracks = await _unitOfWork.Tracks
                    .GetAllAsync(t => !t.IsDeleted, "Enrollments,Enrollments.Payments");

                var revenueByTrack = tracks
                    .Select(t =>
                    {
                        var relevantEnrollments = t.Enrollments?
                                                 .Where(e => e.Status != EnrollmentStatus.Cancelled)
                                                 .ToList() ?? new List<Enrollment>();

                        var enrollmentCount = relevantEnrollments.Count;
                        var expectedRevenue = t.Price * enrollmentCount;
                        var totalPaid = relevantEnrollments
                            .SelectMany(e => e.Payments)
                            .Where(p => p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                            .Sum(p => p.Amount);

                        return new ReportRevenueByTrackResponse
                        {
                            EnrollmentCount = enrollmentCount,
                            TotalRevenue = expectedRevenue,
                            TotalPaid = totalPaid,
                            Outstanding = Math.Max(0, expectedRevenue - totalPaid)
                        };
                    })
                    .OrderByDescending(r => r.TotalRevenue)
                    .ToList();

                if (!revenueByTrack.Any()) return Result<PagedResult<ReportRevenueByTrackResponse>>.FailureResult("No Tracks are found.", ".NotFound");


                var totalCount = revenueByTrack.Count;
                var items = revenueByTrack
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Result<PagedResult<ReportRevenueByTrackResponse>>.SuccessResult(new PagedResult<ReportRevenueByTrackResponse>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Top tracks retrieved successfully.");
               
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ReportRevenueByTrackResponse>>.FailureResult("Error retrieving revenue by track.", ex.Message);
            }
        }

        public async Task<Result<List<TrackMiniDetailsResponse>>> GetTopTrackAsync(int topCount = 5)
        {
            try
            {
                if (topCount < 1) 
                    return Result<List<TrackMiniDetailsResponse>>.FailureResult("Validation errors.", "The count must be a positive number.");

                var tracks = (await _unitOfWork.Tracks.GetAsync(null,t => new TrackMiniDetailsResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Level = t.Level,
                        Status = t.Status,
                        EnrolledCount = t.Enrollments.Where(e=>e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed).Count()

                    }, includes: "Enrollments")).OrderByDescending(t => t.EnrolledCount).Take(topCount).ToList();

                if(!tracks.Any()) return Result<List<TrackMiniDetailsResponse>>.FailureResult("No tracks are found.", ".NotFound");

                return Result<List<TrackMiniDetailsResponse>>.SuccessResult(tracks, "Revenue by track retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<List<TrackMiniDetailsResponse>>.FailureResult("Error retrieving revenue by track.", ex.Message);
            }
        }

        public async Task<Result<List<InstructorWorkLoadResponse>>> GetInstructorWorkLoadAsync()
        {
            try
            {
                var instructors = (await _unitOfWork.Instructors
                        .GetAsync(selector:i => new InstructorWorkLoadResponse
                        {
                            Id = i.Id,
                            FullName = i.FullName,
                            Email = i.Email,
                            ActiveStudents = i.TrainingTracks.SelectMany(t => t.Enrollments).Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed),
                            TrackCount = i.TrainingTracks.Count()
                        })).ToList();

                if (!instructors.Any()) return Result<List<InstructorWorkLoadResponse>>.FailureResult("No instructors are found.", ".NotFound");

                return Result<List<InstructorWorkLoadResponse>>.SuccessResult(instructors, "Instructors workload retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<List<InstructorWorkLoadResponse>>.FailureResult( "Error retrieving revenue by track.", ex.Message);

            }
        }

        public async Task<Result<List<StudentsWithoutPayment>>> GetStudentsWithoutPaymentsAsync()
        {
            try
            {
                var students = (await _unitOfWork.Students
                             .GetAsync(s => !s.IsDeleted && s.Enrollments.Any(e => e.Status == EnrollmentStatus.Draft
                                 && !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Paid
                                                      || p.PaymentStatus == PaymentStatus.PartiallyPaid)),
                             s => new StudentsWithoutPayment
                             {
                                 Id = s.Id,
                                 FullName = s.FullName,
                                 Email = s.Email,
                             }))
                             .ToList();

                if (!students.Any()) return Result<List<StudentsWithoutPayment>>.FailureResult("No tracks are found.", ".NotFound");


                return Result<List<StudentsWithoutPayment>>.SuccessResult(students, "Instructors workload retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<List<StudentsWithoutPayment>>.FailureResult("Error retrieving revenue by track.", ex.Message);
            }
        }
    }
}