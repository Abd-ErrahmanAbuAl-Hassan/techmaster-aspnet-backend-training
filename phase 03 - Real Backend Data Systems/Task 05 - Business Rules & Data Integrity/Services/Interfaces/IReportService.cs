using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Utilities;

namespace Task_05_Business_Rules_Data_Integrity.Services.Interfaces
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