using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Utilities;

namespace Task_05_Business_Rules_Data_Integrity.Services.Interfaces
{
    public interface IStudentService
    {
        Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, string? search = null, bool? isActive = null, bool? isDeleted = null);
        Task<ApiResponse<StudentDetailsResponse>> GetStudentByIdAsync(int id);
        Task<ApiResponse<StudentDetailsResponse>> CreateStudentAsync(CreateStudentRequest request);
        Task<ApiResponse<StudentDetailsResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request);
        Task<ApiResponse<string>> DeleteStudentAsync(int id);
    }
}
