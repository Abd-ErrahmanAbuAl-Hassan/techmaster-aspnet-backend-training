using Microsoft.Extensions.Logging;
using TrainingCenter.Application.DTOs.Track.Responses;
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
    /// <summary>
    /// Service for managing instructor-related operations including retrieval, creation, and updates.
    /// </summary>
    public class InstructorService : IInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InstructorService> _logger;

        public InstructorService(IUnitOfWork unitOfWork, ILogger<InstructorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PagedResult<InstructorBasicResponse>>> GetInstructorsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching instructors. Page: {PageNumber}, Size: {PageSize}",
                    pageNumber, pageSize);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetInstructorsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<InstructorBasicResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var query = await _unitOfWork.Instructors.GetAllAsync(i => i.IsActive);
                var totalCount = query.Count();
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
                {
                    _logger.LogInformation("No active instructors found.");
                    return Result<PagedResult<InstructorBasicResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} instructors.", instructors.Count);

                return Result<PagedResult<InstructorBasicResponse>>.SuccessResult(
                    new PagedResult<InstructorBasicResponse>
                    {
                        Items = instructors,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Instructors"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving instructors. Exception: {@Exception}", ex);
                return Result<PagedResult<InstructorBasicResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<InstructorBasicResponse>> GetInstructorByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching instructor with ID: {InstructorId}", id);

                if (id < 1)
                {
                    _logger.LogWarning("Invalid instructor ID: {InstructorId}", id);
                    return Result<InstructorBasicResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID") });
                }

                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == id && i.IsActive);
                if (instructor == null)
                {
                    _logger.LogInformation("Instructor not found. ID: {InstructorId}", id);
                    return Result<InstructorBasicResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Instructor"));
                }

                _logger.LogInformation("Successfully retrieved instructor. ID: {InstructorId}", id);

                return Result<InstructorBasicResponse>.SuccessResult(
                    new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    },
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Instructor"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving instructor by ID: {InstructorId}. Exception: {@Exception}", id, ex);
                return Result<InstructorBasicResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<TrackDetailsResponse>>> GetInstructorTracksAsync(int instructorId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching instructor tracks. InstructorId: {InstructorId}, Page: {PageNumber}, Size: {PageSize}",
                    instructorId, pageNumber, pageSize);

                var errors = new List<string>();
                if (instructorId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID"));
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetInstructorTracksAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<TrackDetailsResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == instructorId && i.IsActive);
                if (instructor == null)
                {
                    _logger.LogInformation("Instructor not found. ID: {InstructorId}", instructorId);
                    return Result<PagedResult<TrackDetailsResponse>>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Instructor"));
                }

                var query = await _unitOfWork.Tracks.GetAllAsync(
                    t => t.InstructorId == instructorId && !t.IsDeleted,
                    "Enrollments");

                var totalCount = query.Count();
                var tracks = query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!tracks.Any())
                {
                    _logger.LogInformation("No tracks found for instructor. InstructorId: {InstructorId}", instructorId);
                    return Result<PagedResult<TrackDetailsResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

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

                _logger.LogInformation("Successfully retrieved {Count} tracks for instructor. InstructorId: {InstructorId}", response.Count, instructorId);

                return Result<PagedResult<TrackDetailsResponse>>.SuccessResult(
                    new PagedResult<TrackDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Instructor tracks retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving instructor tracks. InstructorId: {InstructorId}, Exception: {@Exception}", instructorId, ex);
                return Result<PagedResult<TrackDetailsResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<InstructorBasicResponse>> CreateInstructorAsync(CreateInstructorRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new instructor. Request: {@Request}", new { request.Email, request.PhoneNumber });

                var errors = new List<string>();

                if (request == null)
                {
                    _logger.LogWarning("CreateInstructorRequest is null.");
                    return Result<InstructorBasicResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { "Request cannot be null." });
                }

                errors = UserValidation.Validate(request);

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for CreateInstructorAsync. Errors: {@Errors}", errors);
                    return Result<InstructorBasicResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                if (await _unitOfWork.Instructors.ExistsAsync(i => i.Email == request.Email))
                {
                    _logger.LogWarning("Email already exists for another instructor. Email: {Email}", request.Email);
                    return Result<InstructorBasicResponse>.ConflictResult(
                        ResultMessages.Conflict.DuplicateEmail);
                }

                if (await _unitOfWork.Instructors.ExistsAsync(i => i.PhoneNumber == request.PhoneNumber))
                {
                    _logger.LogWarning("Phone number already exists for another instructor. Phone: {Phone}", request.PhoneNumber);
                    return Result<InstructorBasicResponse>.ConflictResult(
                        ResultMessages.Conflict.DuplicatePhoneNumber);
                }

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

                _logger.LogInformation("Successfully created instructor. ID: {InstructorId}, Email: {Email}", instructor.Id, instructor.Email);

                return Result<InstructorBasicResponse>.SuccessResult(
                    new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    },
                    string.Format(ResultMessages.Success.ResourceCreated, "Instructor"),
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating instructor. Exception: {@Exception}", ex);
                return Result<InstructorBasicResponse>.FailureResult(
                    ResultMessages.ServerError.DatabaseError,
                    ex.Message);
            }
        }

        public async Task<Result<InstructorBasicResponse>> UpdateInstructorAsync(int id, UpdateInstructorRequest request)
        {
            try
            {
                _logger.LogInformation("Updating instructor. ID: {InstructorId}", id);

                if (id < 1)
                {
                    _logger.LogWarning("Invalid instructor ID: {InstructorId}", id);
                    return Result<InstructorBasicResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID") });
                }

                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == id);
                if (instructor == null)
                {
                    _logger.LogInformation("Instructor not found for update. ID: {InstructorId}", id);
                    return Result<InstructorBasicResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Instructor"));
                }

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != instructor.Email)
                {
                    if (await _unitOfWork.Instructors.ExistsAsync(i => i.Email == request.Email && i.Id != id))
                    {
                        _logger.LogWarning("Email already exists for another instructor. Email: {Email}", request.Email);
                        return Result<InstructorBasicResponse>.ConflictResult(
                            ResultMessages.Conflict.DuplicateEmail);
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != instructor.PhoneNumber)
                {
                    if (await _unitOfWork.Instructors.ExistsAsync(i => i.PhoneNumber == request.PhoneNumber && i.Id != id))
                    {
                        _logger.LogWarning("Phone number already exists for another instructor. Phone: {Phone}", request.PhoneNumber);
                        return Result<InstructorBasicResponse>.ConflictResult(
                            ResultMessages.Conflict.DuplicatePhoneNumber);
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.FName))
                    instructor.FName = request.FName;
                if (!string.IsNullOrWhiteSpace(request.LName))
                    instructor.LName = request.LName;
                if (!string.IsNullOrWhiteSpace(request.Email))
                    instructor.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.Specialization))
                    instructor.Specialization = request.Specialization;
                if (!string.IsNullOrWhiteSpace(request.Bio))
                    instructor.Bio = request.Bio;
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                    instructor.PhoneNumber = request.PhoneNumber;
                if (request.IsActive.HasValue)
                    instructor.IsActive = request.IsActive.Value;

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully updated instructor. ID: {InstructorId}", id);

                return Result<InstructorBasicResponse>.SuccessResult(
                    new InstructorBasicResponse
                    {
                        Id = instructor.Id,
                        FullName = instructor.FullName,
                        Email = instructor.Email
                    },
                    string.Format(ResultMessages.Success.ResourceUpdated, "Instructor"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating instructor. ID: {InstructorId}, Exception: {@Exception}", id, ex);
                return Result<InstructorBasicResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }
    }
}
