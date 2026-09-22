using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Utilities;

namespace Task_05_Business_Rules_Data_Integrity.Services.Interfaces
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
