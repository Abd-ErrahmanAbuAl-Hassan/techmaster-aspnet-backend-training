using TrainingCenter.Application.DTOs.User.Requests;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface IStudentService
    {
        Task<Result<PagedResult<StudentListItemResponse>>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, string? search = null, bool? isActive = null, bool? isDeleted = null);
        Task<Result<StudentDetailsResponse>> GetStudentByIdAsync(int id);
        Task<Result<StudentDetailsResponse>> CreateStudentAsync(CreateStudentRequest request);
        Task<Result<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request);
        Task<Result<string>> DeleteStudentAsync(int id);
    }
}
