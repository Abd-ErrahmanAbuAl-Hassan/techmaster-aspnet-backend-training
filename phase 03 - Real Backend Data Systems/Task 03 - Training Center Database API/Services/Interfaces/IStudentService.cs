using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Utilities;

namespace Task_03___Training_Center_Database_API.Services.Interfaces
{
    public interface IStudentService
    {
        Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, string? search = null, bool? isActive = null);
        Task<ApiResponse<StudentDetailsResponse>> GetStudentByIdAsync(int id);
        Task<ApiResponse<StudentDetailsResponse>> CreateStudentAsync(CreateStudentRequest request);
        Task<ApiResponse<StudentDetailsResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request);
        Task<ApiResponse<string>> DeleteStudentAsync(int id);
    }
}
