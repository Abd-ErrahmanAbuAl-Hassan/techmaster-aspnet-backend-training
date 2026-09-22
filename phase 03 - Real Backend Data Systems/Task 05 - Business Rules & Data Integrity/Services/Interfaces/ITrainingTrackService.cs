using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services.Interfaces
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
