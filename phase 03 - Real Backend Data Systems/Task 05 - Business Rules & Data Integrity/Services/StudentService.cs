using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, string? search = null, bool? isActive = null, bool? isDeleted = null)
        {
            try
            {
                var query = _context.Students.AsQueryable();

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

                var totalCount = await query.CountAsync();
                var students = await query
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
                    .ToListAsync();

                if (!students.Any())
                    return new ApiResponse<PagedResult<StudentListItemResponse>>
                    {
                        Success = false,
                        Message = "No Students are found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                return new ApiResponse<PagedResult<StudentListItemResponse>>
                {
                    Success = true,
                    Message = "Students retrieved successfully.",
                    Data = new PagedResult<StudentListItemResponse>
                    {
                        Items = students,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<StudentListItemResponse>>
                {
                    Success = false,
                    Message = "Error retrieving students.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<StudentDetailsResponse>> GetStudentByIdAsync(int id)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.Enrollments!)
                        .ThenInclude(e => e.TrainingTrack)
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

                if (student == null)
                    return new ApiResponse<StudentDetailsResponse>
                    {
                        Success = false,
                        Message = "Student not found.",
                        ErrorCode = 404
                    };

                var response = new StudentDetailsResponse
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    IsActive = student.IsActive,
                    CreatedAt = student.CreatedAt,
                    UpdatedAt = student.UpdatedAt,
                    EnrollmentCount = student.Enrollments?.Count ?? 0,
                    Enrollments = student.Enrollments?.Select(e => new EnrollmentSummaryResponse
                    {
                        Id = e.Id,
                        TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                        Status = e.Status,
                        EnrollmentDate = e.EnrollmentDate,
                        PaymentStatus = GetPaymentStatus(e.Id)
                    }).ToList() ?? new List<EnrollmentSummaryResponse>()
                };

                return new ApiResponse<StudentDetailsResponse>
                {
                    Success = true,
                    Message = "Student retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<StudentDetailsResponse>
                {
                    Success = false,
                    Message = "Error retrieving student.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<StudentDetailsResponse>> CreateStudentAsync(CreateStudentRequest request)
        {
            try
            {
                var errors = ValidateStudentRequest(request);
                if (errors.Any())
                    return new ApiResponse<StudentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                if (await _context.Students.AnyAsync(s => s.Email == request.Email))
                    return new ApiResponse<StudentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Email already exists." }
                    };

                var student = new Student
                {
                    FName = request.FName,
                    LName = request.LName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true
                    //CreatedAt = DateTime.UtcNow
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

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

                return new ApiResponse<StudentDetailsResponse>
                {
                    Success = true,
                    Message = "Student created successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<StudentDetailsResponse>
                {
                    Success = false,
                    Message = "Error creating student.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<StudentDetailsResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
                if (student == null)
                    return new ApiResponse<StudentDetailsResponse>
                    {
                        Success = false,
                        Message = "Student not found.",
                        ErrorCode = 404
                    };

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != student.Email)
                {
                    if (await _context.Students.AnyAsync(s => s.Email == request.Email && s.Id != id))
                        return new ApiResponse<StudentDetailsResponse>
                        {
                            Success = false,
                            Message = "Validation errors.",
                            ErrorCode = 400,
                            Errors = new List<string> { "Email already exists." }
                        };
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

                //student.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var response = new StudentDetailsResponse
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    IsActive = student.IsActive,
                    CreatedAt = student.CreatedAt,
                    UpdatedAt = student.UpdatedAt,
                    EnrollmentCount = student.Enrollments?.Count() ?? 0,
                    Enrollments = new List<EnrollmentSummaryResponse>()
                };

                return new ApiResponse<StudentDetailsResponse>
                {
                    Success = true,
                    Message = "Student updated successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<StudentDetailsResponse>
                {
                    Success = false,
                    Message = "Error updating student.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteStudentAsync(int id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
                if (student == null)
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Student not found.",
                        ErrorCode = 404
                    };

                student.IsActive = false;
                student.IsDeleted = true;
                student.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Student deleted successfully.",
                    Data = $"Student {id} has been soft deleted."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error deleting student.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private List<string> ValidateStudentRequest(CreateStudentRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.FName))
                errors.Add("First name is required.");
            if (string.IsNullOrWhiteSpace(request.LName))
                errors.Add("Last name is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!IsValidEmail(request.Email))
                errors.Add("Email format is invalid.");
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                errors.Add("Phone number is required.");
            else if (!IsValidPhoneNumber(request.PhoneNumber))
                errors.Add("Phone number format is invalid.");

            return errors;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^01[0125]\d{8}$");
        }

        private PaymentStatus GetPaymentStatus(int enrollmentId)
        {
            var totalPrice = _context.Enrollments
                .Include(e => e.TrainingTrack)
                .FirstOrDefault(e => e.Id == enrollmentId)?.TrainingTrack?.Price ?? 0;

            var totalPaid = _context.Payments
                .Where(p => p.EnrollmentId == enrollmentId && p.PaymentStatus == PaymentStatus.Paid)
                .Sum(p => p.Amount);

            if (totalPaid >= totalPrice)
                return PaymentStatus.Paid;
            else if (totalPaid > 0)
                return PaymentStatus.PartiallyPaid;
            else
                return PaymentStatus.Pending;
        }
    }
}
