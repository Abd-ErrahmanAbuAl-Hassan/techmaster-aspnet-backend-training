using Microsoft.EntityFrameworkCore;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ReportDashboardResponse>> GetDashboardSummaryAsync()
        {
            try
            {
                var totalStudents = await _context.Students.CountAsync(s => s.IsActive);
                var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);
                var completedEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Completed);
                var totalTracks = await _context.TrainingTracks.CountAsync(t => !t.IsDeleted);

                var collected = await _context.Payments
                                .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                         || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                .SumAsync(p => p.Amount);

                var refunded = await _context.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Refunded)
                    .SumAsync(p => p.Amount);

                var totalRevenue = collected - refunded;

                var unpaidAmount = await _context.Enrollments
                                .Where(e => e.Status == EnrollmentStatus.Draft)
                                .Select(e => new
                                {
                                    e.TrainingTrack!.Price,
                                    Paid = e.Payments
                                        .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                                 || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                        .Sum(p => p.Amount)
                                })
                                .Select(x => x.Price - x.Paid)
                                .Where(owed => owed > 0)   // clamp at enrollment level, not grand total
                                .SumAsync();

                var response = new ReportDashboardResponse
                {
                    TotalStudents = totalStudents,
                    ActiveEnrollments = activeEnrollments,
                    CompletedEnrollments = completedEnrollments,
                    TotalTracks = totalTracks,
                    TotalRevenue = totalRevenue,
                    UnpaidAmount = unpaidAmount > 0 ? unpaidAmount : 0
                };

                return new ApiResponse<ReportDashboardResponse>
                {
                    Success = true,
                    Message = "Dashboard summary retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ReportDashboardResponse>
                {
                    Success = false,
                    Message = "Error retrieving dashboard summary.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>> GetUnpaidEnrollmentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };


                var unpaidEnrollments = await _context.Enrollments
                            .Where(e => e.Status == EnrollmentStatus.Draft)
                            .Select(e => new ReportUnpaidEnrollmentResponse
                            {
                                EnrollmentId = e.Id,
                                StudentTitle = e.Student.FullName ?? string.Empty,
                                TrackTitle = e.TrainingTrack.Title ?? string.Empty,
                                TrackPrice = e.TrainingTrack.Price ,
                                TotalPaid = PaymentCalculator.Calculate(e).TotalPaid,
                                EnrollmentDate = e.EnrollmentDate
                            })
                            .Where(x => x.TotalPaid < x.TrackPrice)
                            .OrderByDescending(x => x.EnrollmentDate).ToListAsync();

                if (!unpaidEnrollments.Any()) return new ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>
                {
                    Success = false,
                    Message = "No unpaid enrollments are found.",
                    ErrorCode = 404
                };

                var totalCount = unpaidEnrollments.Count();
                var items = unpaidEnrollments
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                return new ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>
                {
                    Success = true,
                    Message = "Unpaid enrollments retrieved successfully.",
                    Data = new PagedResult<ReportUnpaidEnrollmentResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>
                {
                    Success = false,
                    Message = "Error retrieving unpaid enrollments.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<ReportTrackCapacityResponse>>> GetTrackCapacityAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<ReportTrackCapacityResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var tracks = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                    .Where(t => !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var capacityReports = tracks
                    .Select(t => new ReportTrackCapacityResponse
                    {
                        TrackId = t.Id,
                        TrackTitle = t.Title,
                        Capacity = t.Capacity,
                        Enrolled = t.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed) ?? 0
                    })
                    .ToList();

                if (!capacityReports.Any()) return new ApiResponse<PagedResult<ReportTrackCapacityResponse>>
                {
                    Success = false,
                    Message = "No Tracks are found.",
                    ErrorCode = 404
                };

                var totalCount = capacityReports.Count;
                var items = capacityReports
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new ApiResponse<PagedResult<ReportTrackCapacityResponse>>
                {
                    Success = true,
                    Message = "Track capacity report retrieved successfully.",
                    Data = new PagedResult<ReportTrackCapacityResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ReportTrackCapacityResponse>>
                {
                    Success = false,
                    Message = "Error retrieving track capacity report.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<ReportTrackAvailableSeatsResponse>>> GetTracksWithAvailableSeatsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<ReportTrackAvailableSeatsResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var tracks = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                    .Where(t => !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

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

                if (!availableSeats.Any()) return new ApiResponse<PagedResult<ReportTrackAvailableSeatsResponse>>
                {
                    Success = false,
                    Message = "No Tracks with available are found.",
                    ErrorCode = 404
                };

                var totalCount = availableSeats.Count;
                var items = availableSeats
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new ApiResponse<PagedResult<ReportTrackAvailableSeatsResponse>>
                {
                    Success = true,
                    Message = "Tracks with available seats retrieved successfully.",
                    Data = new PagedResult<ReportTrackAvailableSeatsResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ReportTrackAvailableSeatsResponse>>
                {
                    Success = false,
                    Message = "Error retrieving tracks with available seats.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync()
        {
            try
            {
                var collected = await _context.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                    .SumAsync(p => p.Amount);

                var refunded = await _context.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Refunded)
                    .SumAsync(p => p.Amount);

                var paidAmount = await _context.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Paid)
                    .SumAsync(p => p.Amount);

                var paidCount = await _context.Payments
                    .CountAsync(p => p.PaymentStatus == PaymentStatus.Paid);

                var totalCount = await _context.Payments.CountAsync();

                var totalRevenue = collected - refunded;

                var partiallyPaidAmount = await _context.Payments.Where(p => p.PaymentStatus == PaymentStatus.PartiallyPaid).SumAsync(p => p.Amount);
                var pendingAmount = await _context.Enrollments
                                .Where(e => e.Status == EnrollmentStatus.Draft)
                                .Select(e => new
                                {
                                    e.TrainingTrack!.Price,
                                    Paid = e.Payments
                                        .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                                 || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                        .Sum(p => p.Amount)
                                })
                                .Select(x => x.Price - x.Paid)
                                .Where(owed => owed > 0)   // clamp at enrollment level, not grand total
                                .SumAsync();

                var response = new ReportRevenueSummaryResponse
                {
                    TotalRevenue = totalRevenue,
                    PaidAmount = paidAmount,
                    PartiallyPaidAmount = partiallyPaidAmount,
                    PendingAmount = pendingAmount,
                    TotalPayments = totalCount,
                    PaidCount = paidCount
                };

                return new ApiResponse<ReportRevenueSummaryResponse>
                {
                    Success = true,
                    Message = "Revenue summary retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ReportRevenueSummaryResponse>
                {
                    Success = false,
                    Message = "Error retrieving revenue summary.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<ReportRevenueByTrackResponse>>> GetRevenueByTrackAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<ReportRevenueByTrackResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var tracks = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                        .ThenInclude(e => e.Payments)
                    .Where(t => !t.IsDeleted)
                    .ToListAsync();

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

                if (!revenueByTrack.Any()) return new ApiResponse<PagedResult<ReportRevenueByTrackResponse>>
                {
                    Success = false,
                    Message = "No Tracks are found.",
                    ErrorCode = 404
                };

                var totalCount = revenueByTrack.Count;
                var items = revenueByTrack
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new ApiResponse<PagedResult<ReportRevenueByTrackResponse>>
                {
                    Success = true,
                    Message = "Top tracks retrieved successfully.",
                    Data = new PagedResult<ReportRevenueByTrackResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ReportRevenueByTrackResponse>>
                {
                    Success = false,
                    Message = "Error retrieving revenue by track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<TrackMiniDetailsResponse>>> GetTopTrackAsync(int topCount = 5)
        {
            try
            {
                if (topCount < 1) 
                    return new ApiResponse<List<TrackMiniDetailsResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "The count must be a positive number."}
                    };

                var tracks = await _context.TrainingTracks.Include(t => t.Enrollments)
                    .Select(t => new TrackMiniDetailsResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Level = t.Level,
                        Status = t.Status,
                        EnrolledCount = t.Enrollments.Where(e=>e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed).Count()

                    }).OrderByDescending(t => t.EnrolledCount).Take(topCount).ToListAsync();

                if(!tracks.Any()) return new ApiResponse<List<TrackMiniDetailsResponse>>
                {
                    Success = false,
                    Message = "No tracks are found.",
                    ErrorCode = 404
                };

                return new ApiResponse<List<TrackMiniDetailsResponse>>
                {
                    Success = true,
                    Message = "Revenue by track retrieved successfully.",
                    Data = tracks
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TrackMiniDetailsResponse>>
                {
                    Success = false,
                    Message = "Error retrieving revenue by track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<InstructorWorkLoadResponse>>> GetInstructorWorkLoadAsync()
        {
            try
            {
                var instructors = await _context.Instructors
                        .Select(i => new InstructorWorkLoadResponse
                        {
                            Id = i.Id,
                            FullName = i.FullName,
                            Email = i.Email,
                            ActiveStudents = i.TrainingTracks.SelectMany(t => t.Enrollments).Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed),
                            TrackCount = i.TrainingTracks.Count()
                        }).ToListAsync();

                if (!instructors.Any()) return new ApiResponse<List<InstructorWorkLoadResponse>>
                {
                    Success = false,
                    Message = "No instructors are found.",
                    ErrorCode = 404
                };

                return new ApiResponse<List<InstructorWorkLoadResponse>>
                {
                    Success = true,
                    Message = "Instructors workload retrieved successfully.",
                    Data = instructors
                };

            }
            catch (Exception ex)
            {
                return new ApiResponse<List<InstructorWorkLoadResponse>>
                {
                    Success = false,
                    Message = "Error retrieving revenue by track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<StudentsWithoutPayment>>> GetStudentsWithoutPaymentsAsync()
        {
            try
            {
                var students = await _context.Students
                             .Where(s => !s.IsDeleted)
                             .Where(s => s.Enrollments.Any(e => e.Status == EnrollmentStatus.Draft
                                 && !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Paid
                                                      || p.PaymentStatus == PaymentStatus.PartiallyPaid)))
                             .Select(s => new StudentsWithoutPayment
                             {
                                 Id = s.Id,
                                 FullName = s.FullName,
                                 Email = s.Email,
                             })
                             .ToListAsync();

                if (!students.Any()) return new ApiResponse<List<StudentsWithoutPayment>>
                {
                    Success = false,
                    Message = "No tracks are found.",
                    ErrorCode = 404
                };

                return new ApiResponse<List<StudentsWithoutPayment>>
                {
                    Success = true,
                    Message = "Instructors workload retrieved successfully.",
                    Data = students
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<StudentsWithoutPayment>>
                {
                    Success = false,
                    Message = "Error retrieving revenue by track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}