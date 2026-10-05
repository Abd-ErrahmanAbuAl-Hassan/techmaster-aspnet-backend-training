using TrainingCenter.Application.DTOs.Report.Responses;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<Result<ReportDashboardResponse>> GetDashboardSummaryAsync();
        Task<Result<PagedResult<ReportUnpaidEnrollmentResponse>>> GetUnpaidEnrollmentsAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<PagedResult<ReportTrackCapacityResponse>>> GetTrackCapacityAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<PagedResult<ReportTrackAvailableSeatsResponse>>> GetTracksWithAvailableSeatsAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<ReportRevenueSummaryResponse>> GetRevenueSummaryAsync();
        Task<Result<PagedResult<ReportRevenueByTrackResponse>>> GetRevenueByTrackAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<List<TrackMiniDetailsResponse>>> GetTopTrackAsync(int topCount = 5);
        Task<Result<List<InstructorWorkLoadResponse>>> GetInstructorWorkLoadAsync();
        Task<Result<List<StudentsWithoutPayment>>> GetStudentsWithoutPaymentsAsync();

    }
}