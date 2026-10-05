using TrainingCenter.Application.DTOs.Track.Requests;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface ITrainingTrackService
    {
        Task<Result<PagedResult<TrackDetailsResponse>>> GetTracksAsync(int pageNumber = 1, int pageSize = 10, string? keyword = null, TrackLevel? level = null, TrackStatus? status = null, int? instructorId = null);
        Task<Result<TrackDetailsResponse>> GetTrackByIdAsync(int id);
        Task<Result<TrackDetailsResponse>> CreateTrackAsync(CreateTrackRequest request);
        Task<Result<TrackDetailsResponse>> UpdateTrackAsync(int id, UpdateTrackRequest request);
        Task<Result<string>> DeleteTrackAsync(int id, int instructorId);
    }
}
