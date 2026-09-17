using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Services.Interfaces
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
