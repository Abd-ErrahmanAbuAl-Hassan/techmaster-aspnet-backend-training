using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Task_03___Training_Center_Database_API.Data;
using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Entities;
using Task_03___Training_Center_Database_API.Services.Interfaces;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly ApplicationDbContext _context;

        public InstructorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<InstructorBasicResponse>>> GetInstructorsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Instructors.Where(i => i.IsActive);
                var totalCount = await query.CountAsync();
                var instructors = await query
                    .OrderBy(i => i.FName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new InstructorBasicResponse
                    {
                        Id = i.Id,
                        FullName = i.FullName,
                        Email = i.Email
                    })
                    .ToListAsync();

                if (!instructors.Any())
                    return new ApiResponse<PagedResult<InstructorBasicResponse>>
                    {
                        Success = false,
                        Message = "No instructors are found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                return new ApiResponse<PagedResult<InstructorBasicResponse>>
                {
                    Success = true,
                    Message = "Instructors retrieved successfully.",
                    Data = new PagedResult<InstructorBasicResponse>
                    {
                        Items = instructors,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<InstructorBasicResponse>>
                {
                    Success = false,
                    Message = "Error retrieving instructors.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<InstructorBasicResponse>> GetInstructorByIdAsync(int id)
        {
            try
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == id && i.IsActive);
                if (instructor == null)
                    return new ApiResponse<InstructorBasicResponse>
                    {
                        Success = false,
                        Message = "Instructor not found.",
                        ErrorCode = 404
                    };

                return new ApiResponse<InstructorBasicResponse>
                {
                    Success = true,
                    Message = "Instructor retrieved successfully.",
                    Data = new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<InstructorBasicResponse>
                {
                    Success = false,
                    Message = "Error retrieving instructor.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<TrackDetailsResponse>>> GetInstructorTracksAsync(int instructorId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == instructorId && i.IsActive);
                if (instructor == null)
                    return new ApiResponse<PagedResult<TrackDetailsResponse>>
                    {
                        Success = false,
                        Message = "Instructor not found.",
                        ErrorCode = 404
                    };

                var query = _context.TrainingTracks
                    .Include(t => t.Enrollments)
                    .Where(t => t.InstructorId == instructorId && (t.IsDeleted!= null && !t.IsDeleted.Value));

                var totalCount = await query.CountAsync();
                var tracks = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!tracks.Any())
                    return new ApiResponse<PagedResult<TrackDetailsResponse>>
                    {
                        Success = false,
                        Message = "No tracks are found for instructor.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                var response = tracks.Select(t => new TrackDetailsResponse
                {
                    Id = t.Id,
                    Name = t.Title,
                    Description = t.Description,
                    Level = t.Level,
                    Status = t.Status,
                    Price = t.Price,
                    Capacity = t.Capacity,
                    EnrolledCount = t.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed) ?? 0,
                    Instructor = new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    },
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList();

                return new ApiResponse<PagedResult<TrackDetailsResponse>>
                {
                    Success = true,
                    Message = "Instructor tracks retrieved successfully.",
                    Data = new PagedResult<TrackDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<TrackDetailsResponse>>
                {
                    Success = false,
                    Message = "Error retrieving instructor tracks.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        public async Task<ApiResponse<InstructorBasicResponse>> CreateInstructorAsync(CreateInstructorRequest request)
        {
            try
            {
                var errors = new List<string>();

                if (request == null)
                    return new ApiResponse<InstructorBasicResponse>
                    {
                        Success = false,
                        Message = "Validation error.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Request cannot be null." }
                    };

                if (string.IsNullOrWhiteSpace(request.FName)) errors.Add("First name is required.");
                if (string.IsNullOrWhiteSpace(request.LName)) errors.Add("Last name is required.");
                if (string.IsNullOrWhiteSpace(request.Email)) errors.Add("Email is required.");
                else if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Email format is invalid.");
                if (string.IsNullOrWhiteSpace(request.Specialization)) errors.Add("Specialization is required.");
                if (string.IsNullOrWhiteSpace(request.Bio)) errors.Add("Bio is required.");

                if (errors.Any())
                    return new ApiResponse<InstructorBasicResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                if (await _context.Instructors.AnyAsync(i => i.Email == request.Email))
                    return new ApiResponse<InstructorBasicResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Email already exists." }
                    };

                var instructor = new Instructor
                {
                    FName = request.FName,
                    LName = request.LName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber!,
                    Specialization = request.Specialization,
                    Bio = request.Bio,
                    IsActive = true
                    //CreatedAt = DateTime.UtcNow
                };

                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();

                return new ApiResponse<InstructorBasicResponse>
                {
                    Success = true,
                    Message = "Instructor created successfully.",
                    Data = new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<InstructorBasicResponse>
                {
                    Success = false,
                    Message = "Error creating instructor.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<InstructorBasicResponse>> UpdateInstructorAsync(int id, UpdateInstructorRequest request)
        {
            try
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == id);
                if (instructor == null)
                    return new ApiResponse<InstructorBasicResponse>
                    {
                        Success = false,
                        Message = "Instructor not found.",
                        ErrorCode = 404
                    };

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != instructor.Email)
                {
                    if (await _context.Instructors.AnyAsync(i => i.Email == request.Email && i.Id != id))
                        return new ApiResponse<InstructorBasicResponse>
                        {
                            Success = false,
                            Message = "Validation errors.",
                            ErrorCode = 400,
                            Errors = new List<string> { "Email already exists." }
                        };
                }

                if (!string.IsNullOrWhiteSpace(request.FName)) instructor.FName = request.FName;
                if (!string.IsNullOrWhiteSpace(request.LName)) instructor.LName = request.LName;
                if (!string.IsNullOrWhiteSpace(request.Email)) instructor.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.Specialization)) instructor.Specialization = request.Specialization;
                if (!string.IsNullOrWhiteSpace(request.Bio)) instructor.Bio = request.Bio;
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) instructor.PhoneNumber = request.PhoneNumber;
                if (request.IsActive.HasValue) instructor.IsActive = request.IsActive.Value;

                //instructor.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ApiResponse<InstructorBasicResponse>
                {
                    Success = true,
                    Message = "Instructor updated successfully.",
                    Data = new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<InstructorBasicResponse>
                {
                    Success = false,
                    Message = "Error updating instructor.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
