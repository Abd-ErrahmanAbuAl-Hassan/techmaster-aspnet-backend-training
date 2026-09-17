using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Utilities;

namespace Task_03___Training_Center_Database_API.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<ApiResponse<PagedResult<InstructorBasicResponse>>> GetInstructorsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<InstructorBasicResponse>> GetInstructorByIdAsync(int id);
        Task<ApiResponse<PagedResult<TrackDetailsResponse>>> GetInstructorTracksAsync(int instructorId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<InstructorBasicResponse>> CreateInstructorAsync(CreateInstructorRequest request);
        Task<ApiResponse<InstructorBasicResponse>> UpdateInstructorAsync(int id, UpdateInstructorRequest request);
    }
}
