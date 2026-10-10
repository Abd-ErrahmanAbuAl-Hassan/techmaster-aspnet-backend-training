using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using TrainingCenter.Application.DTOs.Track.Requests;
using TrainingCenter.Application.DTOs.Track.Responses;
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
    public class TrainingTrackService : ITrainingTrackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TrainingTrackService> _logger;
        public TrainingTrackService(IUnitOfWork unitOfWork, ILogger<TrainingTrackService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PagedResult<TrackDetailsResponse>>> GetTracksAsync(int pageNumber = 1, int pageSize = 10, string? keyword = null, TrackLevel? level = null, TrackStatus? status = null, int? instructorId = null)
        {
            try
            {
                _logger.LogInformation("Fetching training tracks. Page: {PageNumber}, Size: {PageSize}, Keyword: {Keyword}, Level: {Level}, Status: {Status}, InstructorId: {InstructorId}",
                                   pageNumber, pageSize, keyword, level, status, instructorId);

                var errors = new List<string>();

                if (instructorId.HasValue && instructorId.Value < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID"));
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");
                if (level.HasValue && !Enum.IsDefined(typeof(TrackLevel), level))
                    errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Track level"));
                if (status.HasValue && !Enum.IsDefined(typeof(TrackStatus), status))
                    errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Track status"));

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetTracksAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<TrackDetailsResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var query = await _unitOfWork.Tracks.GetAllAsync(t => !t.IsDeleted, includes: "Instructor,Enrollments");

                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(t => t.Title.Contains(keyword) || t.Description.Contains(keyword));
                if (level.HasValue)
                    query = query.Where(t => t.Level == level.Value);
                if (status.HasValue)
                    query = query.Where(t => t.Status == status.Value);
                if (instructorId.HasValue)
                    query = query.Where(t => t.InstructorId == instructorId.Value);

                var totalCount = query.Count();
                var tracks = query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!tracks.Any())
                {
                    _logger.LogInformation("No training tracks found with the specified criteria.");
                    return Result<PagedResult<TrackDetailsResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                var response = tracks.Select(t => MapToTrackDetailsResponse(t)).ToList();

                _logger.LogInformation("Successfully retrieved {Count} training tracks.", response.Count);

                return Result<PagedResult<TrackDetailsResponse>>.SuccessResult(
                    new PagedResult<TrackDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Tracks"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving training tracks. Exception: {@Exception}", ex);
                return Result<PagedResult<TrackDetailsResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,ex.Message);
            }
        }
        public async Task<Result<TrackDetailsResponse>> GetTrackByIdAsync(int id)
        {
            try
            {
                if (id < 1) return Result<TrackDetailsResponse>.FailureResult("Validation Errors.", "Track ID must be positive number.");

                var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, "Instructor,Enrollments");

                if (track == null)
                    return Result<TrackDetailsResponse>.FailureResult("Track not found.", ".NotFound");


                return Result<TrackDetailsResponse>.SuccessResult(MapToTrackDetailsResponse(track), "Track retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<TrackDetailsResponse>.FailureResult("Error retrieving track.", ex.Message);

            }
        }
        public async Task<Result<TrackDetailsResponse>> CreateTrackAsync(CreateTrackRequest request)
        {
            try
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Title)) errors.Add("Track name is required.");
                if (string.IsNullOrWhiteSpace(request.Description)) errors.Add("Track description is required.");
                if (request.Capacity <= 0) errors.Add("Capacity must be greater than zero.");
                if (request.InstructorId < 1) errors.Add("Instructor ID must be positive number.");
                if (request.Price <= 0) errors.Add("Price must be greater than zero.");
                if (request.StartDate >= request.EndDate) errors.Add("StartDate must be before EndDate.");
                if (!Enum.IsDefined(typeof(TrackLevel), request.Level)) errors.Add("Track level is invalid.");

                var instructor = await _unitOfWork.Instructors.GetFirstOrDefaultAsync(i => i.Id == request.InstructorId);
                if (!(request.InstructorId < 1) && instructor == null)
                    errors.Add("Instructor not found.");

                if (errors.Any())
                    return Result<TrackDetailsResponse>.FailureResult("Validation errors.", errors);

                string code;
                var attempts = 0;
                do
                {
                    code = $"{RandomNumberGenerator.GetInt32(0, 100000):D6}";
                    attempts++;
                }
                while (await _unitOfWork.Tracks.ExistsAsync(t => t.Code == code) && attempts < 10);

                if (await _unitOfWork.Tracks.ExistsAsync(t => t.Code == code))
                    return Result<TrackDetailsResponse>.FailureResult("Validation Errors.", "Could not generate a unique track code. Please try again.");

                var track = new TrainingTrack
                {
                    Title = request.Title,
                    Code = code,
                    Description = request.Description,
                    InstructorId = request.InstructorId,
                    Capacity = request.Capacity,
                    Price = request.Price,
                    Level = request.Level,
                    Status = TrackStatus.Published,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    IsActive = true
                };

                await _unitOfWork.Tracks.AddAsync(track);
                await _unitOfWork.SaveAsync();

                track = await _unitOfWork.Tracks

                    .GetFirstOrDefaultAsync(t => t.Id == track.Id, "Instructor,Enrollments");

                return Result<TrackDetailsResponse>.SuccessResult(MapToTrackDetailsResponse(track), "Track created successfully.");

            }
            catch (Exception ex)
            {
                return Result<TrackDetailsResponse>.FailureResult("Error creating track.", ex.Message);

            }
        }
        public async Task<Result<TrackDetailsResponse>> UpdateTrackAsync(int id, UpdateTrackRequest request)
        {
            try
            {
                var errors = new List<string>();
                if (id < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));
                if (request.InstructorId < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID"));
                if (request.Level.HasValue && !Enum.IsDefined(typeof(TrackLevel), request.Level.Value)) errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Track level"));
                if (request.StartDate.HasValue != request.EndDate.HasValue) errors.Add(string.Format(ResultMessages.Validation.RequiredField,"Both Date"));
                if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate.Value > request.EndDate.Value) errors.Add(ResultMessages.Validation.DateRangeInvalid);
                if (request.Capacity.HasValue && request.Capacity <= 0) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Capacity"));
                if (request.Price.HasValue && request.Price <= 0) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Price"));

                var track = await _unitOfWork.Tracks
                    .GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, "Instructor,Enrollments");

                if (track == null)
                {
                    _logger.LogInformation("Training track not found for update. ID: {TrackId}", id);
                    return Result<TrackDetailsResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track"));
                }

                if (track.InstructorId != request.InstructorId)
                    return Result<TrackDetailsResponse>.ForbiddenResult(ResultMessages.Unauthorized.AccessDenied);

                var effectiveStart = request.StartDate ?? track.StartDate;
                var effectiveEnd = request.EndDate ?? track.EndDate;
                if (effectiveStart >= effectiveEnd)
                    errors.Add(ResultMessages.Validation.DateRangeInvalid);
                if (request.Capacity.HasValue && request.Capacity.Value < track.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed))
                    errors.Add("Capacity cannot be less than the number of currently enrolled students.");
                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for UpdateTrackAsync. Errors: {@Errors}", errors);
                    return Result<TrackDetailsResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                if (!string.IsNullOrWhiteSpace(request.Title))
                    track.Title = request.Title;
                if (!string.IsNullOrWhiteSpace(request.Description))
                    track.Description = request.Description;
                if (request.Capacity.HasValue)
                    track.Capacity = request.Capacity.Value;
                if (request.Price.HasValue)
                    track.Price = request.Price.Value;
                if (request.Level.HasValue && request.Level > 0)
                    track.Level = request.Level.Value;
                if (request.StartDate.HasValue)
                    track.StartDate = request.StartDate.Value;
                if (request.EndDate.HasValue)
                    track.EndDate = request.EndDate.Value;

                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Successfully updated training track. ID: {TrackId}", id);
                return Result<TrackDetailsResponse>.SuccessResult(
                                    MapToTrackDetailsResponse(track),
                                    string.Format(ResultMessages.Success.ResourceUpdated, "Training track"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating training track. ID: {TrackId}, Exception: {@Exception}", id, ex);
                return Result<TrackDetailsResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }
        public async Task<Result> DeleteTrackAsync(int id, int InstructorId)
        {
            try
            {

                var errors = new List<string>();
                if (id < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));
                if (InstructorId < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID"));

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for DeleteTrackAsync. Errors: {@Errors}", errors);
                    return Result.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }
                var track = await _unitOfWork.Tracks
                    .GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, "Enrollments");

                if (track == null)
                {
                    _logger.LogInformation("Training track not found for deletion. ID: {TrackId}", id);
                    return Result.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track"));
                }

                if (track.InstructorId != InstructorId)
                    return Result.ForbiddenResult(ResultMessages.Unauthorized.AccessDenied);


                var activeEnrollments = track.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
                if (activeEnrollments > 0)
                {
                    _logger.LogWarning("Cannot delete track with active enrollments. TrackId: {TrackId}, ActiveEnrollments: {Count}", id, activeEnrollments);
                    return Result.ConflictResult(
                        "Cannot delete a track that has active or draft enrollments. Please close all enrollments first.");
                }

                track.IsDeleted = true;
                track.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully deleted training track. ID: {TrackId}", id);

                return Result.SuccessResult(
                    string.Format(ResultMessages.Success.ResourceDeleted, "Training track"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting training track. ID: {TrackId}, Exception: {@Exception}", id, ex);
                return Result.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }
        public async Task<Result> AssignInstructorToTrackAsync(int id, int instructorId)
        {
            var errors = new List<string>();
            if (id < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));
            if (instructorId < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID"));

            if (errors.Any())
            {
                _logger.LogWarning("Validation failed for Assign Instructor To Track. Errors: {@Errors}", errors);
                return Result.ValidationErrorResult(
                    "Validation errors occurred. Please review the details below.",
                    errors);
            }
            var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (track == null)
            {
                _logger.LogInformation("Training track not found for assignment. ID: {TrackId}", id);
                return Result.NotFoundResult(
                    string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track"));
            }
            var instructor = await _unitOfWork.Users.GetByIdAsync(instructorId);
            if (instructor == null)
            {
                _logger.LogInformation("Instructor not found for assignment. ID: {InstructorId}", instructorId);
                return Result.NotFoundResult(
                    string.Format(ResultMessages.NotFound.ResourceNotFound, "Instructor"));
            }
            if (!instructor.IsActive)
            {
                _logger.LogInformation("Instructor is inactive for assignment. ID: {InstructorId}", instructorId);
                return Result.ConflictResult(ResultMessages.Conflict.InvalidUserStatus);
            }
            if (track.InstructorId == instructorId)
            {
                _logger.LogInformation("Instructor is already assigned. ID: {InstructorId}", instructorId);
                return Result.ConflictResult(ResultMessages.Conflict.AlreadyAssigned);
            }
            track.InstructorId = instructorId;
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Successfully assign instructor to the training track. ID: {InstructorId}", instructorId);

            return Result.SuccessResult(ResultMessages.Success.RegistrationSuccess);

        }
        public async Task<Result<BasicTrackSessionResponse>> CreateTrackSessionAsync(int trackId, CreateTrackSessionRequest request)
        {
            var errors = TrackValidation.ValidateTrackSession(request);
            if (trackId < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));
            if (errors.Any())
                return Result<BasicTrackSessionResponse>.ValidationErrorResult(ResultMessages.Validation.NonNegativeNumber, errors);

            var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == trackId && !t.IsDeleted);

            if (track == null)
            {
                _logger.LogInformation("Training track not found for deletion. ID: {TrackId}", trackId);
                return Result<BasicTrackSessionResponse>.NotFoundResult(
                    string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track"));
            }
            if (!track.IsActive)
            {
                _logger.LogInformation("Training track not active for session creation. ID: {TrackId}", trackId);
                return Result<BasicTrackSessionResponse>.NotFoundResult(
                    ResultMessages.Conflict.InvalidTrackStatus);
            }
            var instructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId!.Value);

            if (instructor == null)
                return Result<BasicTrackSessionResponse>.NotFoundResult(ResultMessages.NotFound.InstructorNotFound);
            if (!instructor.IsActive)
            {
                _logger.LogInformation("Instructor is inactive for session creation. ID: {InstructorId}", instructor.Id);
                return Result<BasicTrackSessionResponse>.ConflictResult(ResultMessages.Conflict.InvalidUserStatus);
            }
            if (track.InstructorId != request.InstructorId)
                return Result<BasicTrackSessionResponse>.ForbiddenResult(ResultMessages.Unauthorized.AccessDenied);

            var session = new TrackSession
            {
                Title = request.Title,
                Description = request.Description,
                MeetingLink = request.MeetingLink,
                SessionDate = request.SessionDate,
                CreatedByInstructorId = request.InstructorId.Value,
                TrackId = trackId
            };

            await _unitOfWork.TrackSessions.AddAsync(session);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Successfully create session to the training track. ID: {trackId}", trackId);

            return Result<BasicTrackSessionResponse>.SuccessResult(new BasicTrackSessionResponse
            {
                SessionId = session.Id,
                Title = session.Title,
                MeetingLink = session.MeetingLink,
                SessionDate = session.SessionDate
            },string.Format(ResultMessages.Success.ResourceCreated, "Training track"),201);
        }
        public async Task<Result<DetailedTrackSessionResponse>> UpdateTrackSessionAsync(int sessionId, UpdateSessionRequest request)
        {
            var errors = new List<string>();
            if (sessionId < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));
            if (request.InstructorId < 1) errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Instructor ID"));
            if (request.MeetingLink != null && !TrackValidation.TryValidateUrl(request.MeetingLink, out var error)) errors.Add(error!);
            if (errors.Any())
            {
                _logger.LogWarning("Validation failed for Update Track Session. Errors: {@Errors}", errors);
                return Result<DetailedTrackSessionResponse>.ValidationErrorResult(
                    "Validation errors occurred. Please review the details below.",
                    errors);
            }
            var session = await _unitOfWork.TrackSessions.GetFirstOrDefaultAsync(t => t.Id == sessionId);

            if (session == null)
            {
                _logger.LogInformation("Training track session not found for update. ID: {SessionId}", sessionId);
                return Result<DetailedTrackSessionResponse>.NotFoundResult(
                    string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track session"));
            }
            var instructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId!);

            if (instructor == null)
                return Result<DetailedTrackSessionResponse>.NotFoundResult(ResultMessages.NotFound.InstructorNotFound);
            if (!instructor.IsActive)
            {
                _logger.LogInformation("Instructor is inactive for session update. ID: {InstructorId}", instructor.Id);
                return Result<DetailedTrackSessionResponse>.ConflictResult(ResultMessages.Conflict.InvalidUserStatus);
            }
            if (session.CreatedByInstructorId != request.InstructorId)
                return Result<DetailedTrackSessionResponse>.ForbiddenResult(ResultMessages.Unauthorized.AccessDenied);

            session.Title = request.Title ?? session.Title;
            session.Description = request.Description ?? session.Description;
            session.MeetingLink = request.MeetingLink ?? session.MeetingLink;
            session.SessionDate = request.SessionDate ?? session.SessionDate;

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Successfully update session to the training track. ID: {trackId}", sessionId);

            return Result<DetailedTrackSessionResponse>.SuccessResult(new DetailedTrackSessionResponse
            {
                SessionId = session.Id,
                Title = session.Title,
                Description = session.Description,
                MeetingLink = session.MeetingLink,
                SessionDate = session.SessionDate,
                Instructor = new InstructorBasicResponse
                {
                    Id = instructor.Id,
                    FullName = instructor.FullName,
                    Email = instructor.Email
                }
            },ResultMessages.Success.ResourceUpdated);
        }
        public async Task<Result<TrackProgressResponse>> GetTrackProgressAsync(int trackId, int instructorId)
        {
            if (trackId < 1 || instructorId < 1)
                return Result<TrackProgressResponse>.ValidationErrorResult(string.Format(ResultMessages.Validation.PositiveNumber,"ID"),null!);
            
            var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == trackId && !t.IsDeleted,includes:"Enrollments");

            if (track == null)
                return Result<TrackProgressResponse>.NotFoundResult(ResultMessages.NotFound.TrackNotFound);
            if (!track.IsActive)
                return Result<TrackProgressResponse>.ConflictResult(ResultMessages.Conflict.InvalidTrackStatus);

           if (track.InstructorId != instructorId)
                return Result<TrackProgressResponse>.ForbiddenResult(ResultMessages.Unauthorized.AccessDenied);

           var enrollmentCount = track.Enrollments?.Count ?? 0;
           var progressPercentage = track.Enrollments?.Sum(e=>e.ProgressPercentage) ?? 0;
            _logger.LogInformation("Successfully get training track progress. ID: {TrackId}", trackId);
            return Result<TrackProgressResponse>.SuccessResult(new TrackProgressResponse
            {
                TrackTitle = track.Title,
                EnrollmentCount = enrollmentCount,
                ProgressPercentage = progressPercentage
            },ResultMessages.Success.ResourceRetrieved);

        }

        private TrackDetailsResponse MapToTrackDetailsResponse(TrainingTrack track)
        {
            var enrolledCount = track.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed) ?? 0;
            return new TrackDetailsResponse
            {
                Id = track.Id,
                Title = track.Title,
                Description = track.Description,
                Level = track.Level,
                Status = track.Status,
                Price = track.Price,
                Capacity = track.Capacity,
                EnrolledCount = enrolledCount,
                Instructor = new InstructorBasicResponse
                {
                    Id = track.Instructor!.Id,
                    FullName = track.Instructor.FullName,
                    Email = track.Instructor.Email
                },
                CreatedAt = track.CreatedAt,
                UpdatedAt = track.UpdatedAt
            };
        }

    }
}
