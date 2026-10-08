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
        Task<Result> DeleteTrackAsync(int id, int instructorId);
        Task<Result> AssignInstructorToTrackAsync(int id, int instructorId);
        Task<Result<BasicTrackSessionResponse>> CreateTrackSessionAsync(int trackId, CreateTrackSessionRequest request);
        Task<Result<DetailedTrackSessionResponse>> UpdateTrackSessionAsync(int trackId, UpdateSessionRequest request);
        Task<Result<TrackProgressResponse>> GetTrackProgressAsync(int trackId, int instructorId);

    }
}
