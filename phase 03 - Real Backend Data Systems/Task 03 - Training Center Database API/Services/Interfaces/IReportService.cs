using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Utilities;

namespace Task_03___Training_Center_Database_API.Services.Interfaces
{
    public interface IReportService
    {
        Task<ApiResponse<ReportDashboardResponse>> GetDashboardSummaryAsync();
        Task<ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>> GetUnpaidEnrollmentsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<ReportTrackCapacityResponse>>> GetTrackCapacityAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync();
        Task<ApiResponse<PagedResult<ReportRevenueByTrackResponse>>> GetRevenueByTrackAsync(int pageNumber = 1, int pageSize = 10);
    }
}