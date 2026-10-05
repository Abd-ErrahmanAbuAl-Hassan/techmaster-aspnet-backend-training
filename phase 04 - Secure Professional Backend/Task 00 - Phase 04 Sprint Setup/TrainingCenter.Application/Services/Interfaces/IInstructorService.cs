using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Requests;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<Result<PagedResult<InstructorBasicResponse>>> GetInstructorsAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<InstructorBasicResponse>> GetInstructorByIdAsync(int id);
        Task<Result<PagedResult<TrackDetailsResponse>>> GetInstructorTracksAsync(int instructorId, int pageNumber = 1, int pageSize = 10);
        Task<Result<InstructorBasicResponse>> CreateInstructorAsync(CreateInstructorRequest request);
        Task<Result<InstructorBasicResponse>> UpdateInstructorAsync(int id, UpdateInstructorRequest request);
    }
}
