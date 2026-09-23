using Microsoft.EntityFrameworkCore;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services
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

                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<InstructorBasicResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

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
                if(id < 1 ) return new ApiResponse<InstructorBasicResponse>
                {
                    Success = false,
                    Message = "Validation Errors.",
                    ErrorCode = 400,
                    Errors = new List<string> { "Instructor id must be positive number." }
                };


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
                var errors = new List<string>();
                if (instructorId < 1) errors.Add("Track ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<TrackDetailsResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

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
                    .Where(t => t.InstructorId == instructorId && !t.IsDeleted);

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
                    Title = t.Title,
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

                errors = PersonValidator.Validate(request);
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

                if (await _context.Instructors.AnyAsync(i => i.PhoneNumber == request.PhoneNumber))
                    return new ApiResponse<InstructorBasicResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Phone number already exists." }
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
                if(id < 1) return new ApiResponse<InstructorBasicResponse>
                {
                    Success = false,
                    Message = "Validation Errors.",
                    ErrorCode = 400,
                    Errors = new List<string> { "Instructor id must be positive number." }
                };

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

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != instructor.PhoneNumber)
                {
                    if (await _context.Instructors.AnyAsync(i => i.PhoneNumber == request.PhoneNumber && i.Id != id))
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
