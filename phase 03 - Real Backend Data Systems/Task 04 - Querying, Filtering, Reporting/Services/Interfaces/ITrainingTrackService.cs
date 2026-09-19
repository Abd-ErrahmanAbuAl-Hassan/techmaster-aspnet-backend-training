using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.DTOs.Responses;
using Task_04_Querying_Filtering_Reporting.Utilities;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Services.Interfaces
{
    public interface ITrainingTrackService
    {
        Task<ApiResponse<PagedResult<TrackDetailsResponse>>> GetTracksAsync(int pageNumber = 1, int pageSize = 10, string? keyword = null, TrackLevel? level = null, TrackStatus? status = null, int? instructorId = null);
        Task<ApiResponse<TrackDetailsResponse>> GetTrackByIdAsync(int id);
        Task<ApiResponse<TrackDetailsResponse>> CreateTrackAsync(CreateTrackRequest request);
        Task<ApiResponse<TrackDetailsResponse>> UpdateTrackAsync(int id, UpdateTrackRequest request);
        Task<ApiResponse<string>> DeleteTrackAsync(int id, int instructorId);
    }
}
