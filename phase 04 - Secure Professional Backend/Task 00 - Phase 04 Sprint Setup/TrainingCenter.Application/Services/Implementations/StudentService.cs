using TrainingCenter.Application.DTOs.Enrollment.Responses;
using TrainingCenter.Application.DTOs.User.Requests;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Application.Helpers;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Application.Validations;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<StudentListItemResponse>>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, string search = null, bool? isActive = null, bool? isDeleted = null)
        {
            try
            {
                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<StudentListItemResponse>>.FailureResult("Validation errors.", errors);

                var query = await _unitOfWork.Students.GetAllAsync();

                if (isActive.HasValue)
                    query = query.Where(s => s.IsActive == isActive.Value);

                if (isDeleted.HasValue)
                    query = query.Where(s => s.IsDeleted == isDeleted.Value);
                else
                    query = query.Where(s => !s.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(s => s.Email.Contains(search.Trim())
                                            || (s.FName + "" + s.LName).Contains(search.Trim())
                                            || s.PhoneNumber.Contains(search.Trim()));

                var totalCount = query.Count();
                var students = query
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new StudentListItemResponse
                    {
                        Id = s.Id,
                        FullName = s.FullName,
                        Email = s.Email,
                        PhoneNumber = s.PhoneNumber,
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt
                    })
                    .ToList();

                if (!students.Any())
                    return Result<PagedResult<StudentListItemResponse>>.FailureResult("No Students are found.", ".NotFound");
              

                return Result<PagedResult<StudentListItemResponse>>.SuccessResult(new PagedResult<StudentListItemResponse>
                {
                    Items = students,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Students retrieved successfully.");
              
            }
            catch (Exception ex)
            {
                return Result<PagedResult<StudentListItemResponse>>.FailureResult("Error retrieving students.", ex.Message);
            
            }
        }
        public async Task<Result<StudentDetailsResponse>> GetStudentByIdAsync(int id)
        {
            try
            {
                if (id < 1) return Result<StudentDetailsResponse>.FailureResult("Validation errors.", "Student ID must be positive number." );


                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, "Enrollments,Enrollments.TrainingTrack,Enrollments.Payments");
                    
                if (student == null)
                    return Result<StudentDetailsResponse>.FailureResult("Student not found.", ".NotFound");
              

                var response = new StudentDetailsResponse
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    IsActive = student.IsActive,
                    CreatedAt = student.CreatedAt,
                    UpdatedAt = student.UpdatedAt,
                    EnrollmentCount = student.Enrollments?.Where(e=>e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed).Count() ?? 0,
                    Enrollments = student.Enrollments?.Select(e => new EnrollmentSummaryResponse
                    {
                        Id = e.Id,
                        TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                        Status = e.Status,
                        EnrollmentDate = e.EnrollmentDate,
                        PaymentStatus = PaymentCalculator.Calculate(e).Status
                    }).ToList() ?? new List<EnrollmentSummaryResponse>()
                };

                return Result<StudentDetailsResponse>.SuccessResult(response, "Student retrieved successfully.");
             
            }
            catch (Exception ex)
            {
                return Result<StudentDetailsResponse>.FailureResult("Error retrieving student.", ex.Message);
               
            }
        }
        public async Task<Result<StudentDetailsResponse>> CreateStudentAsync(CreateStudentRequest request)
        {
            try
            {
                var errors = UserValidation.Validate(request);
                if (errors.Any())
                    return Result<StudentDetailsResponse>.FailureResult("Validation errors.", errors);

                if (await _unitOfWork.Students.ExistsAsync(s => s.Email == request.Email))
                    return Result<StudentDetailsResponse>.FailureResult("Validation Errors.", "Email already exists.");
             

                if (await _unitOfWork.Students.ExistsAsync(s => s.PhoneNumber == request.PhoneNumber))
                    return Result<StudentDetailsResponse>.FailureResult("Validation Errors.", "Phone already exists.");
                

                var student = new Student
                {
                    FName = request.FName,
                    LName = request.LName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true
                };

                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveAsync();

                var response = new StudentDetailsResponse
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    IsActive = student.IsActive,
                    CreatedAt = student.CreatedAt,
                    EnrollmentCount = 0,
                    Enrollments = new List<EnrollmentSummaryResponse>()
                };

                return Result<StudentDetailsResponse>.SuccessResult(response, "Student created successfully.");
                
            }
            catch (Exception ex)
            {
                return Result<StudentDetailsResponse>.FailureResult("Error creating student.", ex.Message);
             
            }
        }
        public async Task<Result<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
        {
            try
            {
                if (id < 1) return Result<StudentResponse>.FailureResult("Validation Errors.", "Student ID must be positive number.");
            
                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
                if (student == null)
                    return Result<StudentResponse>.FailureResult("Student not found.", ".NotFound");
            

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != student.Email)
                {
                    if (await _unitOfWork.Students.ExistsAsync(s => s.Email == request.Email && s.Id != id))
                        return Result<StudentResponse>.FailureResult("Validation Errors.","Email already exists." );
                  
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != student.PhoneNumber)
                {
                    if (await _unitOfWork.Students.ExistsAsync(s => s.PhoneNumber == request.PhoneNumber && s.Id != id))
                        return Result<StudentResponse>.FailureResult("Validation Errors.", "Phone number already exists.");
                   
                }

                if (!string.IsNullOrWhiteSpace(request.FName))
                    student.FName = request.FName;
                if (!string.IsNullOrWhiteSpace(request.LName))
                    student.LName = request.LName;
                if (!string.IsNullOrWhiteSpace(request.Email))
                    student.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                    student.PhoneNumber = request.PhoneNumber;
                if (request.IsActive.HasValue)
                    student.IsActive = request.IsActive.Value;

                await _unitOfWork.SaveAsync();

                var response = new StudentResponse
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    IsActive = student.IsActive,
                    CreatedAt = student.CreatedAt,
                    UpdatedAt = student.UpdatedAt
                };

                return Result<StudentResponse>.SuccessResult(response, "Student updated successfully.");
               
            }
            catch (Exception ex)
            {
                return Result<StudentResponse>.FailureResult("Error updating student.", ex.Message);
              
            }
        }
        public async Task<Result<string>> DeleteStudentAsync(int id)
        {
            try
            {
                if (id < 1) return Result<string>.FailureResult("Validation Errors.","Student ID must be positive number." );
            
                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
                if (student == null)
                    return Result<string>.FailureResult("Student not found.", ".NotFound");
            

                student.IsActive = false;
                student.IsDeleted = true;
                student.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();

                return Result<string>.SuccessResult($"Student {id} has been soft deleted.", "Student deleted successfully.");
          
            }
            catch (Exception ex)
            {
                return Result<string>.FailureResult("Error deleting student.", ex.Message);
             
            }
        }
    }
}
