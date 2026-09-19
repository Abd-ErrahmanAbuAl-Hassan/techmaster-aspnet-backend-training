using Task_04_Querying_Filtering_Reporting.DTOs.Responses;
using Task_04_Querying_Filtering_Reporting.Utilities;

namespace Task_04_Querying_Filtering_Reporting.Services.Interfaces
{
    public interface IReportService
    {
        Task<ApiResponse<ReportDashboardResponse>> GetDashboardSummaryAsync();
        Task<ApiResponse<PagedResult<ReportUnpaidEnrollmentResponse>>> GetUnpaidEnrollmentsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<ReportTrackCapacityResponse>>> GetTrackCapacityAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<ReportTrackAvailableSeatsResponse>>> GetTracksWithAvailableSeatsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync();
        Task<ApiResponse<PagedResult<ReportRevenueByTrackResponse>>> GetRevenueByTrackAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<List<TrackMiniDetailsResponse>>> GetTopTrackAsync(int topCount = 5);
        Task<ApiResponse<List<InstructorWorkLoadResponse>>> GetInstructorWorkLoadAsync();
        Task<ApiResponse<List<StudentsWithoutPayment>>> GetStudentsWithoutPaymentsAsync();

    }
}