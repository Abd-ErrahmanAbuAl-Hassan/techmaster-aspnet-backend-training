using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Requests;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Application.Validations;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class InstructorService : IInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<InstructorBasicResponse>>> GetInstructorsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<InstructorBasicResponse>>.FailureResult("Validation errors.", errors);

                var query = await _unitOfWork.Instructors.GetAllAsync(i => i.IsActive);
                var totalCount =  query.Count();
                var instructors = query
                    .OrderBy(i => i.FName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new InstructorBasicResponse
                    {
                        Id = i.Id,
                        FullName = i.FullName,
                        Email = i.Email
                    })
                    .ToList();

                if (!instructors.Any())
                    return Result<PagedResult<InstructorBasicResponse>>.FailureResult("No instructors are found.", ".NotFound");

                return Result<PagedResult<InstructorBasicResponse>>.SuccessResult(new PagedResult<InstructorBasicResponse>
                {
                    Items = instructors,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Instructors retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<PagedResult<InstructorBasicResponse>>.FailureResult("Error retrieving instructors.", ex.Message);
            }
        }
        public async Task<Result<InstructorBasicResponse>> GetInstructorByIdAsync(int id)
        {
            try
            {
                if(id < 1 ) return Result<InstructorBasicResponse>.FailureResult("Validation errors.",  "Instructor id must be positive number.");
 
                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == id && i.IsActive);
                if (instructor == null)
                    return Result<InstructorBasicResponse>.FailureResult("Instructor not found.", ".NotFound");

                return Result<InstructorBasicResponse>.SuccessResult(new InstructorBasicResponse
                {
                    Id = instructor.Id,
                    FullName = instructor.FullName,
                    Email = instructor.Email
                }, "Instructor retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<InstructorBasicResponse>.FailureResult("Error retrieving instructor.", ex.Message);
            }
        }
        public async Task<Result<PagedResult<TrackDetailsResponse>>> GetInstructorTracksAsync(int instructorId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (instructorId < 1) errors.Add("Track ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<TrackDetailsResponse>>.FailureResult("Validation errors.", errors);

                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == instructorId && i.IsActive);
                if (instructor == null)
                    return Result<PagedResult<TrackDetailsResponse>>.FailureResult("Instructor not found.", ".NotFound");

                var query = await _unitOfWork.Tracks.GetAllAsync(t => t.InstructorId == instructorId && !t.IsDeleted, "Enrollments");

                var totalCount = query.Count();
                var tracks = query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!tracks.Any())
                    return Result<PagedResult<TrackDetailsResponse>>.FailureResult("No tracks are found for instructor.", ".NotFound");

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

                return Result<PagedResult<TrackDetailsResponse>>.SuccessResult(new PagedResult<TrackDetailsResponse>
                {
                    Items = response,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Instructor tracks retrieved successfully.");
        
            }
            catch (Exception ex)
            {
                return Result<PagedResult<TrackDetailsResponse>>.FailureResult("Error retrieving instructor tracks.", ex.Message);
            }
        }
        public async Task<Result<InstructorBasicResponse>> CreateInstructorAsync(CreateInstructorRequest request)
        {
            try
            {
                var errors = new List<string>();

                if (request == null)
                    return Result<InstructorBasicResponse>.FailureResult("Validation errors.","Request cannot be null." );
                errors = UserValidation.Validate(request);

                if (errors.Any())
                    return Result<InstructorBasicResponse>.FailureResult("Validation errors.", errors);

                if (await _unitOfWork.Instructors.ExistsAsync(i => i.Email == request.Email))
                    return Result<InstructorBasicResponse>.FailureResult("Validation errors.","Email already exists." );

                if (await _unitOfWork.Instructors.ExistsAsync(i => i.PhoneNumber == request.PhoneNumber))
                    return Result<InstructorBasicResponse>.FailureResult("Validation errors.","Phone number already exists." );

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

                await _unitOfWork.Instructors.AddAsync(instructor);
                await _unitOfWork.SaveAsync();

                return Result<InstructorBasicResponse>.SuccessResult(new InstructorBasicResponse
                {
                    Id = instructor.Id,
                    FullName = instructor.FullName,
                    Email = instructor.Email
                }, "Instructor created successfully.");
            }
            catch (Exception ex)
            {
                return Result<InstructorBasicResponse>.FailureResult("Error creating instructor.", ex.Message);
            }
        }
        public async Task<Result<InstructorBasicResponse>> UpdateInstructorAsync(int id, UpdateInstructorRequest request)
        {
            try
            {
                if(id < 1) return Result<InstructorBasicResponse>
                        .FailureResult("Validation errors.","Instructor id must be positive number." );

                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == id);
                if (instructor == null)
                    return Result<InstructorBasicResponse>.FailureResult("Instructor not found.", ".NotFound");

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != instructor.Email)
                {
                    if (await _unitOfWork.Instructors.ExistsAsync(i => i.Email == request.Email && i.Id != id))
                        return Result<InstructorBasicResponse>.FailureResult("Validation errors.","Email already exists." );
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != instructor.PhoneNumber)
                {
                    if (await _unitOfWork.Instructors.ExistsAsync(i => i.PhoneNumber == request.PhoneNumber && i.Id != id))
                        return Result<InstructorBasicResponse>.FailureResult("Validation errors.","Email already exists." );
                }

                if (!string.IsNullOrWhiteSpace(request.FName)) instructor.FName = request.FName;
                if (!string.IsNullOrWhiteSpace(request.LName)) instructor.LName = request.LName;
                if (!string.IsNullOrWhiteSpace(request.Email)) instructor.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.Specialization)) instructor.Specialization = request.Specialization;
                if (!string.IsNullOrWhiteSpace(request.Bio)) instructor.Bio = request.Bio;
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) instructor.PhoneNumber = request.PhoneNumber;
                if (request.IsActive.HasValue) instructor.IsActive = request.IsActive.Value;

                await _unitOfWork.SaveAsync();

                return Result<InstructorBasicResponse>.SuccessResult(new InstructorBasicResponse
                {
                    Id = instructor.Id,
                    FullName = instructor.FullName,
                    Email = instructor.Email
                }, "Instructors retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<InstructorBasicResponse>.FailureResult("Error updating instructor.", ex.Message);
            }
        }
    }
}
