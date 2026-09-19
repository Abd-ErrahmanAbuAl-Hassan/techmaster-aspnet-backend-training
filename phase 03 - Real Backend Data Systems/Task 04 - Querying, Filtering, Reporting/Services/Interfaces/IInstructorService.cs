using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.DTOs.Responses;
using Task_04_Querying_Filtering_Reporting.Utilities;

namespace Task_04_Querying_Filtering_Reporting.Services.Interfaces
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
