using Microsoft.EntityFrameworkCore;
using Task_03___Training_Center_Database_API.Data;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Services.Interfaces;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Services
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
                var totalTracks = await _context.TrainingTracks.CountAsync(t => t.IsDeleted != null && !t.IsDeleted.Value);

                var totalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Paid)
                    .SumAsync(p => p.Amount);

                var unpaidAmount = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                        .ThenInclude(e => e.Payments)
                    .SumAsync(t => t.Price * t.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled)
                        - (t.Enrollments
                            .Where(e => e.Status != EnrollmentStatus.Cancelled)
                            .SelectMany(e => e.Payments)
                            .Where(p => p.PaymentStatus == PaymentStatus.Paid)
                            .Sum(p => p.Amount)));

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
                var enrollments = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .Include(e => e.Payments)
                    .Where(e => e.Status == EnrollmentStatus.Active)
                    .ToListAsync();

                var unpaidEnrollments = enrollments
                    .Where(e =>
                    {
                        var trackPrice = e.TrainingTrack?.Price ?? 0;
                        var totalPaid = e.Payments?
                            .Where(p => p.PaymentStatus != PaymentStatus.Pending)
                            .Sum(p => p.Amount) ?? 0;
                        return totalPaid < trackPrice;
                    })
                    .Select(e => new ReportUnpaidEnrollmentResponse
                    {
                        EnrollmentId = e.Id,
                        StudentName = e.Student?.FullName ?? string.Empty,
                        TrackName = e.TrainingTrack?.Title ?? string.Empty,
                        TrackPrice = e.TrainingTrack?.Price ?? 0,
                        TotalPaid = e.Payments?
                            .Where(p => p.PaymentStatus != PaymentStatus.Pending)
                            .Sum(p => p.Amount) ?? 0,
                        EnrollmentDate = e.EnrollmentDate
                    })
                    .OrderByDescending(r => r.EnrollmentDate)
                    .ToList();

                var totalCount = unpaidEnrollments.Count;
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
                var tracks = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                    .Where(t => t.IsDeleted != null && !t.IsDeleted.Value)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var capacityReports = tracks
                    .Select(t => new ReportTrackCapacityResponse
                    {
                        TrackId = t.Id,
                        TrackName = t.Title,
                        Capacity = t.Capacity,
                        Enrolled = t.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0
                    })
                    .ToList();

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

        public async Task<ApiResponse<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync()
        {
            try
            {
                var payments = await _context.Payments.ToListAsync();

                var totalRevenue = payments.Sum(p => p.Amount);
                var paidAmount = payments.Where(p => p.PaymentStatus == PaymentStatus.Paid).Sum(p => p.Amount);
                var partiallyPaidAmount = payments.Where(p => p.PaymentStatus == PaymentStatus.PartiallyPaid).Sum(p => p.Amount);
                var pendingAmount = payments.Where(p => p.PaymentStatus == PaymentStatus.Pending).Sum(p => p.Amount);

                var response = new ReportRevenueSummaryResponse
                {
                    TotalRevenue = totalRevenue,
                    PaidAmount = paidAmount,
                    PartiallyPaidAmount = partiallyPaidAmount,
                    PendingAmount = pendingAmount,
                    TotalPayments = payments.Count,
                    PaidCount = payments.Count(p => p.PaymentStatus == PaymentStatus.Paid)
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
                var tracks = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                        .ThenInclude(e => e.Payments)
                    .Where(t => !t.IsDeleted.Value)
                    .ToListAsync();

                var revenueByTrack = tracks
                    .Select(t =>
                    {
                        var enrollmentCount = t.Enrollments?.Count(e => e.Status != EnrollmentStatus.Cancelled) ?? 0;
                        var totalPaid = t.Enrollments?
                            .Where(e => e.Status != EnrollmentStatus.Cancelled)
                            .SelectMany(e => e.Payments)
                            .Where(p => p.PaymentStatus == PaymentStatus.Paid)
                            .Sum(p => p.Amount) ?? 0;

                        return new ReportRevenueByTrackResponse
                        {
                            TrackId = t.Id,
                            TrackName = t.Title,
                            TrackPrice = t.Price,
                            EnrollmentCount = enrollmentCount,
                            TotalRevenue = t.Price * enrollmentCount,
                            TotalPaid = totalPaid,
                            Outstanding = (t.Price * enrollmentCount) - totalPaid
                        };
                    })
                    .OrderByDescending(r => r.TotalRevenue)
                    .ToList();

                var totalCount = revenueByTrack.Count;
                var items = revenueByTrack
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new ApiResponse<PagedResult<ReportRevenueByTrackResponse>>
                {
                    Success = true,
                    Message = "Revenue by track retrieved successfully.",
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
    }
}