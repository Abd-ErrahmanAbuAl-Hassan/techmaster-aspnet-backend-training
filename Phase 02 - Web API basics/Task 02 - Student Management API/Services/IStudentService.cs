using Task_02___Student_Management_API.DTOs;
using Task_02___Student_Management_API.Entities;
using Task_02___Student_Management_API.Utilities;

namespace Task_02___Student_Management_API.Services
{
    public interface IStudentService
    {
        Result<Student> Create(CreateStudentRequest model);
        Result<List<StudentResponse>> GetAll(Filter? filter = null);
        Result<StudentStatsResponse> GetStats();
        Result<Student> GetById(Guid id);
        Result<Student> GetByTrackName(string trackName);
        Result<Student> Update(Guid id, UpdateStudentRequest model);
        Result<Student> Update(Guid id, UpdateStudentStatusRequest model);
        Result<Student> Delete(Guid id);
    }
}
