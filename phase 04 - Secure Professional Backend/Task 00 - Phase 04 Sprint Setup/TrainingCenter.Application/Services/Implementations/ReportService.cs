using Microsoft.Extensions.Logging;
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
    /// <summary>
    /// Service for generating business reports and analytics including dashboard summaries,
    /// revenue tracking, capacity management, and instructor workload analysis.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<ReportDashboardResponse>> GetDashboardSummaryAsync()
        {
            try
            {
                _logger.LogInformation("Generating dashboard summary report.");

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

                var unpaidAmount = (await _unitOfWork.Enrollments
                                .GetAsync(e => e.Status == EnrollmentStatus.Draft, e => new
                                {
                                    e.TrainingTrack!.Price,
                                    Paid = e.Payments
                                        .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                                 || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                        .Sum(p => p.Amount)
                                })).Select(x => x.Price - x.Paid)
                                .Where(owed => owed > 0)
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

                _logger.LogInformation("Dashboard summary generated successfully. TotalStudents: {TotalStudents}, TotalRevenue: {TotalRevenue}",
                    totalStudents, totalRevenue);

                return Result<ReportDashboardResponse>.SuccessResult(
                    response,
                    "Dashboard summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating dashboard summary. Exception: {@Exception}", ex);
                return Result<ReportDashboardResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportUnpaidEnrollmentResponse>>> GetUnpaidEnrollmentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching unpaid enrollments. Page: {PageNumber}, Size: {PageSize}",
                    pageNumber, pageSize);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetUnpaidEnrollmentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var unpaidEnrollments = (await _unitOfWork.Enrollments
                            .GetAsync(e => e.Status == EnrollmentStatus.Draft, e => new ReportUnpaidEnrollmentResponse
                            {
                                EnrollmentId = e.Id,
                                StudentTitle = e.Student.FullName ?? string.Empty,
                                TrackTitle = e.TrainingTrack.Title ?? string.Empty,
                                TrackPrice = e.TrainingTrack.Price,
                                TotalPaid = PaymentCalculator.Calculate(e).TotalPaid,
                                EnrollmentDate = e.EnrollmentDate
                            }))
                            .Where(x => x.TotalPaid < x.TrackPrice)
                            .OrderByDescending(x => x.EnrollmentDate)
                            .ToList();

                if (!unpaidEnrollments.Any())
                {
                    _logger.LogInformation("No unpaid enrollments found.");
                    return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                var totalCount = unpaidEnrollments.Count();
                var items = unpaidEnrollments
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} unpaid enrollments.", items.Count);

                return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.SuccessResult(
                    new PagedResult<ReportUnpaidEnrollmentResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Unpaid enrollments retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving unpaid enrollments. Exception: {@Exception}", ex);
                return Result<PagedResult<ReportUnpaidEnrollmentResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportTrackCapacityResponse>>> GetTrackCapacityAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching track capacity report. Page: {PageNumber}, Size: {PageSize}",
                    pageNumber, pageSize);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetTrackCapacityAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<ReportTrackCapacityResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var capacityReports = (await _unitOfWork.Tracks
                    .GetAsync(t => !t.IsDeleted, includes: "Enrollments", selector: t => new ReportTrackCapacityResponse
                    {
                        TrackId = t.Id,
                        TrackTitle = t.Title,
                        Capacity = t.Capacity,
                        Enrolled = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed)
                    }))
                    .ToList();

                if (!capacityReports.Any())
                {
                    _logger.LogInformation("No tracks found for capacity report.");
                    return Result<PagedResult<ReportTrackCapacityResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                var totalCount = capacityReports.Count;
                var items = capacityReports
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} track capacity records.", items.Count);

                return Result<PagedResult<ReportTrackCapacityResponse>>.SuccessResult(
                    new PagedResult<ReportTrackCapacityResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Track capacity report retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving track capacity report. Exception: {@Exception}", ex);
                return Result<PagedResult<ReportTrackCapacityResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportTrackAvailableSeatsResponse>>> GetTracksWithAvailableSeatsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching tracks with available seats. Page: {PageNumber}, Size: {PageSize}",
                    pageNumber, pageSize);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetTracksWithAvailableSeatsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var tracks = await _unitOfWork.Tracks.GetAllAsync(t => !t.IsDeleted, "Enrollments");

                var availableSeats = tracks
                    .Select(t =>
                    {
                        var seats = t.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed) ?? 0;
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

                if (!availableSeats.Any())
                {
                    _logger.LogInformation("No tracks with available seats found.");
                    return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.NotFoundResult(
                        "No tracks with available seats are available for enrollment.");
                }

                var totalCount = availableSeats.Count;
                var items = availableSeats
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} tracks with available seats.", items.Count);

                return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.SuccessResult(
                    new PagedResult<ReportTrackAvailableSeatsResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Tracks with available seats retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tracks with available seats. Exception: {@Exception}", ex);
                return Result<PagedResult<ReportTrackAvailableSeatsResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync()
        {
            try
            {
                _logger.LogInformation("Generating revenue summary report.");

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
                var pendingAmount = (await _unitOfWork.Enrollments
                                .GetAsync(e => e.Status == EnrollmentStatus.Draft, e => new
                                {
                                    e.TrainingTrack!.Price,
                                    Paid = e.Payments
                                        .Where(p => p.PaymentStatus == PaymentStatus.Paid
                                                 || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                                        .Sum(p => p.Amount)
                                }))
                                .Select(x => x.Price - x.Paid)
                                .Where(owed => owed > 0)
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

                _logger.LogInformation("Revenue summary generated successfully. TotalRevenue: {TotalRevenue}, PaidAmount: {PaidAmount}",
                    totalRevenue, paidAmount);

                return Result<ReportRevenueSummaryResponse>.SuccessResult(
                    response,
                    "Revenue summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating revenue summary. Exception: {@Exception}", ex);
                return Result<ReportRevenueSummaryResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<ReportRevenueByTrackResponse>>> GetRevenueByTrackAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching revenue by track report. Page: {PageNumber}, Size: {PageSize}",
                    pageNumber, pageSize);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetRevenueByTrackAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<ReportRevenueByTrackResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var tracks = await _unitOfWork.Tracks.GetAllAsync(t => !t.IsDeleted, "Enrollments,Enrollments.Payments");

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

                if (!revenueByTrack.Any())
                {
                    _logger.LogInformation("No tracks found for revenue report.");
                    return Result<PagedResult<ReportRevenueByTrackResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                var totalCount = revenueByTrack.Count;
                var items = revenueByTrack
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Successfully retrieved {Count} revenue by track records.", items.Count);

                return Result<PagedResult<ReportRevenueByTrackResponse>>.SuccessResult(
                    new PagedResult<ReportRevenueByTrackResponse>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Revenue by track report retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving revenue by track report. Exception: {@Exception}", ex);
                return Result<PagedResult<ReportRevenueByTrackResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<List<TrackMiniDetailsResponse>>> GetTopTrackAsync(int topCount = 5)
        {
            try
            {
                _logger.LogInformation("Fetching top {TopCount} tracks by enrollment.", topCount);

                if (topCount < 1)
                {
                    _logger.LogWarning("Invalid top count: {TopCount}", topCount);
                    return Result<List<TrackMiniDetailsResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Top count") });
                }

                var tracks = (await _unitOfWork.Tracks.GetAsync(null, t => new TrackMiniDetailsResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Level = t.Level,
                    Status = t.Status,
                    EnrolledCount = t.Enrollments.Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed).Count()
                }, includes: "Enrollments"))
                .OrderByDescending(t => t.EnrolledCount)
                .Take(topCount)
                .ToList();

                if (!tracks.Any())
                {
                    _logger.LogInformation("No tracks found.");
                    return Result<List<TrackMiniDetailsResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} top tracks.", tracks.Count);

                return Result<List<TrackMiniDetailsResponse>>.SuccessResult(
                    tracks,
                    "Top tracks retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving top tracks. Exception: {@Exception}", ex);
                return Result<List<TrackMiniDetailsResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<List<InstructorWorkLoadResponse>>> GetInstructorWorkLoadAsync()
        {
            try
            {
                _logger.LogInformation("Generating instructor workload report.");

                var instructors = (await _unitOfWork.Instructors
                        .GetAsync(selector: i => new InstructorWorkLoadResponse
                        {
                            Id = i.Id,
                            FullName = i.FullName,
                            Email = i.Email,
                            ActiveStudents = i.TrainingTracks.SelectMany(t => t.Enrollments).Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed),
                            TrackCount = i.TrainingTracks.Count()
                        }))
                        .OrderByDescending(i => i.ActiveStudents)
                        .ToList();

                if (!instructors.Any())
                {
                    _logger.LogInformation("No instructors found.");
                    return Result<List<InstructorWorkLoadResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} instructor workload records.", instructors.Count);

                return Result<List<InstructorWorkLoadResponse>>.SuccessResult(
                    instructors,
                    "Instructor workload report retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating instructor workload report. Exception: {@Exception}", ex);
                return Result<List<InstructorWorkLoadResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<List<StudentsWithoutPayment>>> GetStudentsWithoutPaymentsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching students without payments.");

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
                             .OrderBy(s => s.FullName)
                             .ToList();

                if (!students.Any())
                {
                    _logger.LogInformation("No students without payments found.");
                    return Result<List<StudentsWithoutPayment>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} students without payments.", students.Count);

                return Result<List<StudentsWithoutPayment>>.SuccessResult(
                    students,
                    "Students without payments retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving students without payments. Exception: {@Exception}", ex);
                return Result<List<StudentsWithoutPayment>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }
    }
}