using Microsoft.Extensions.Logging;
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
        private readonly ILogger<StudentService> _logger;

        public StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PagedResult<StudentListItemResponse>>> GetStudentsAsync(
            int pageNumber = 1, 
            int pageSize = 10, 
            string search = null, 
            bool? isActive = null, 
            bool? isDeleted = null)
        {
            try
            {
                _logger.LogInformation("Fetching students. Page: {PageNumber}, Size: {PageSize}, Search: {Search}, IsActive: {IsActive}, IsDeleted: {IsDeleted}",
                    pageNumber, pageSize, search, isActive, isDeleted);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetStudentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<StudentListItemResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var query = await _unitOfWork.Students.GetAllAsync();

                if (isActive.HasValue)
                    query = query.Where(s => s.IsActive == isActive.Value);

                if (isDeleted.HasValue)
                    query = query.Where(s => s.IsDeleted == isDeleted.Value);
                else
                    query = query.Where(s => !s.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(s => s.Email.Contains(search.Trim())
                                            || (s.FName + " " + s.LName).Contains(search.Trim())
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
                {
                    _logger.LogInformation("No students found with the specified criteria.");
                    return Result<PagedResult<StudentListItemResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} students.", students.Count);

                return Result<PagedResult<StudentListItemResponse>>.SuccessResult(
                    new PagedResult<StudentListItemResponse>
                    {
                        Items = students,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Students"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving students. Exception: {@Exception}", ex);
                return Result<PagedResult<StudentListItemResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<StudentDetailsResponse>> GetStudentByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching student with ID: {StudentId}", id);

                if (id < 1)
                {
                    _logger.LogWarning("Invalid student ID: {StudentId}", id);
                    return Result<StudentDetailsResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Student ID") });
                }

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(
                    s => s.Id == id && !s.IsDeleted,
                    "Enrollments,Enrollments.TrainingTrack,Enrollments.Payments");

                if (student == null)
                {
                    _logger.LogInformation("Student not found. ID: {StudentId}", id);
                    return Result<StudentDetailsResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Student"));
                }

                var response = new StudentDetailsResponse
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    IsActive = student.IsActive,
                    CreatedAt = student.CreatedAt,
                    UpdatedAt = student.UpdatedAt,
                    EnrollmentCount = student.Enrollments?.Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed).Count() ?? 0,
                    Enrollments = student.Enrollments?.Select(e => new EnrollmentSummaryResponse
                    {
                        Id = e.Id,
                        TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                        Status = e.Status,
                        EnrollmentDate = e.EnrollmentDate,
                        PaymentStatus = PaymentCalculator.Calculate(e).Status
                    }).ToList() ?? new List<EnrollmentSummaryResponse>()
                };

                _logger.LogInformation("Successfully retrieved student. ID: {StudentId}", id);

                return Result<StudentDetailsResponse>.SuccessResult(
                    response,
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Student"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving student by ID: {StudentId}. Exception: {@Exception}", id, ex);
                return Result<StudentDetailsResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<StudentDetailsResponse>> CreateStudentAsync(CreateStudentRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new student. Request: {@Request}", new { request.Email, request.PhoneNumber });

                var errors = UserValidation.Validate(request);
                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for CreateStudentAsync. Errors: {@Errors}", errors);
                    return Result<StudentDetailsResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                if (await _unitOfWork.Students.ExistsAsync(s => s.Email == request.Email))
                {
                    _logger.LogWarning("Email already exists. Email: {Email}", request.Email);
                    return Result<StudentDetailsResponse>.ConflictResult(
                        ResultMessages.Conflict.DuplicateEmail);
                }

                if (await _unitOfWork.Students.ExistsAsync(s => s.PhoneNumber == request.PhoneNumber))
                {
                    _logger.LogWarning("Phone number already exists. Phone: {Phone}", request.PhoneNumber);
                    return Result<StudentDetailsResponse>.ConflictResult(
                        ResultMessages.Conflict.DuplicatePhoneNumber);
                }

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

                _logger.LogInformation("Successfully created student. ID: {StudentId}, Email: {Email}", student.Id, student.Email);

                return Result<StudentDetailsResponse>.SuccessResult(
                    response,
                    string.Format(ResultMessages.Success.ResourceCreated, "Student"),
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating student. Exception: {@Exception}", ex);
                return Result<StudentDetailsResponse>.FailureResult(
                    ResultMessages.ServerError.DatabaseError,
                    ex.Message);
            }
        }

        public async Task<Result<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
        {
            try
            {
                _logger.LogInformation("Updating student. ID: {StudentId}", id);

                if (id < 1)
                {
                    _logger.LogWarning("Invalid student ID: {StudentId}", id);
                    return Result<StudentResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Student ID") });
                }

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
                if (student == null)
                {
                    _logger.LogInformation("Student not found for update. ID: {StudentId}", id);
                    return Result<StudentResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Student"));
                }

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != student.Email)
                {
                    if (await _unitOfWork.Students.ExistsAsync(s => s.Email == request.Email && s.Id != id))
                    {
                        _logger.LogWarning("Email already exists for another student. Email: {Email}", request.Email);
                        return Result<StudentResponse>.ConflictResult(
                            ResultMessages.Conflict.DuplicateEmail);
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != student.PhoneNumber)
                {
                    if (await _unitOfWork.Students.ExistsAsync(s => s.PhoneNumber == request.PhoneNumber && s.Id != id))
                    {
                        _logger.LogWarning("Phone number already exists for another student. Phone: {Phone}", request.PhoneNumber);
                        return Result<StudentResponse>.ConflictResult(
                            ResultMessages.Conflict.DuplicatePhoneNumber);
                    }
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

                _logger.LogInformation("Successfully updated student. ID: {StudentId}", id);

                return Result<StudentResponse>.SuccessResult(
                    response,
                    string.Format(ResultMessages.Success.ResourceUpdated, "Student"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating student. ID: {StudentId}, Exception: {@Exception}", id, ex);
                return Result<StudentResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<string>> DeleteStudentAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting student. ID: {StudentId}", id);

                if (id < 1)
                {
                    _logger.LogWarning("Invalid student ID: {StudentId}", id);
                    return Result<string>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Student ID") });
                }

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
                if (student == null)
                {
                    _logger.LogInformation("Student not found for deletion. ID: {StudentId}", id);
                    return Result<string>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Student"));
                }

                student.IsActive = false;
                student.IsDeleted = true;
                student.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully deleted student. ID: {StudentId}", id);

                return Result<string>.SuccessResult(
                    string.Empty,
                    string.Format(ResultMessages.Success.ResourceDeleted, "Student"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting student. ID: {StudentId}, Exception: {@Exception}", id, ex);
                return Result<string>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }
    }
}
