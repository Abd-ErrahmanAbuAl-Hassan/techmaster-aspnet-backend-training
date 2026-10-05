using System.Security.Cryptography;
using TrainingCenter.Application.DTOs.Track.Requests;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class TrainingTrackService : ITrainingTrackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainingTrackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<TrackDetailsResponse>>> GetTracksAsync(int pageNumber = 1, int pageSize = 10, string keyword = null, TrackLevel? level = null, TrackStatus? status = null, int? instructorId = null)
        {
            try
            {

                var errors = new List<string>();
                if (instructorId.HasValue && instructorId.Value < 1) errors.Add("Instructor ID must be a positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");
                if (!Enum.IsDefined(typeof(TrackLevel), level)) errors.Add("Track level is invalid.");
                if (!Enum.IsDefined(typeof(TrackStatus), status)) errors.Add("Track status is invalid.");

                if (errors.Any())
                    return Result<PagedResult<TrackDetailsResponse>>.FailureResult("Validation errors.", errors);

                var query = await _unitOfWork.Tracks.GetAllAsync(t => !t.IsDeleted, includes: "Instructor,Enrollments");
                    

                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(t => t.Title.Contains(keyword) || t.Description.Contains(keyword));
                if (level.HasValue)
                    query = query.Where(t => t.Level == level.Value);
                if (status.HasValue)
                    query = query.Where(t => t.Status == status.Value);
                if (instructorId.HasValue)
                    query = query.Where(t => t.InstructorId == instructorId.Value);

                var totalCount =  query.Count();
                var tracks =  query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!tracks.Any())
                    return Result<PagedResult<TrackDetailsResponse>>.FailureResult("No tracks are found.", ".NotFound");
             

                var response = tracks.Select(t => MapToTrackDetailsResponse(t)).ToList();

                return Result<PagedResult<TrackDetailsResponse>>.SuccessResult(new PagedResult<TrackDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },"Tracks retrieved successfully.");
               
            }
            catch (Exception ex)
            {
                return Result<PagedResult<TrackDetailsResponse>>.FailureResult("Error retrieving tracks.", ex.Message);
          
            }
        }
        public async Task<Result<TrackDetailsResponse>> GetTrackByIdAsync(int id)
        {
            try
            {
                if (id < 1) return Result<TrackDetailsResponse>.FailureResult("Validation Errors.","Track ID must be positive number."  );

                var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, "Instructor,Enrollments");
             
                if (track == null)
                    return Result<TrackDetailsResponse>.FailureResult("Track not found.", ".NotFound");
       

                return Result<TrackDetailsResponse>.SuccessResult(MapToTrackDetailsResponse(track),"Track retrieved successfully.");
            
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

                return Result<TrackDetailsResponse>.SuccessResult(MapToTrackDetailsResponse(track),"Track created successfully.");
              
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
                if (id < 1) errors.Add("Track ID must be positive number.");
                if (request.InstructorId < 1) errors.Add("Instructor ID must be positive number.");
                if (request.Level.HasValue && !Enum.IsDefined(typeof(TrackLevel), request.Level.Value)) errors.Add("Track level is invalid.");
                if (request.StartDate.HasValue != request.EndDate.HasValue) errors.Add("Both start and end dates must be provided together.");
                if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate.Value > request.EndDate.Value) errors.Add("Start date must be less than or equal End date.");
                if (request.Capacity.HasValue && request.Capacity <= 0) errors.Add("Capacity must be greater than zero.");
                if (request.Price.HasValue && request.Price <= 0) errors.Add("Price must be greater than zero.");

                var track = await _unitOfWork.Tracks
                    .GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, "Instructor,Enrollments");

                if (track == null)
                    return Result<TrackDetailsResponse>.FailureResult("Track not found.", ".NotFound");
              

                if (track.InstructorId != request.InstructorId)
                    return Result<TrackDetailsResponse>.FailureResult("Access Denied.", $"Instructor id:{request.InstructorId} not allowed to access.");
       
                var effectiveStart = request.StartDate ?? track.StartDate;
                var effectiveEnd = request.EndDate ?? track.EndDate;
                if (effectiveStart >= effectiveEnd)
                    errors.Add("StartDate must be before EndDate.");
                if (request.Capacity.HasValue && request.Capacity.Value < track.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed))
                    errors.Add("Capacity cannot be less than the number of currently enrolled students.");
                if (errors.Any())
                    return Result<TrackDetailsResponse>.FailureResult("Validation Errors.",errors );
              

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

                return Result<TrackDetailsResponse>.SuccessResult( MapToTrackDetailsResponse(track),"Track updated successfully.");
             
            }
            catch (Exception ex)
            {
                return Result<TrackDetailsResponse>.FailureResult("Error updating track.", ex.Message);
            
            }
        }
        public async Task<Result<string>> DeleteTrackAsync(int id, int InstructorId)
        {
            try
            {

                var errors = new List<string>();
                if (id < 1) errors.Add("Track ID must be positive number.");
                if (InstructorId < 1) errors.Add("Instructor ID must be positive number.");

                if (errors.Any())
                    return Result<string>.FailureResult("Validation errors.", errors);
                var track = await _unitOfWork.Tracks
                    .GetFirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, "Enrollments");

                if (track == null)
                    return Result<string>.FailureResult( "Track not found.",".NotFound" );
              
                if (track.InstructorId != InstructorId)
                    return Result<string>.FailureResult("Access Denied.", $"Instructor id:{InstructorId} not allowed to access.");
                

                var activeEnrollments = track.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
                if (activeEnrollments > 0)
                    return Result<string>.FailureResult("Cannot delete track with active enrollments.", $"Track has {activeEnrollments} active enrollment(s).");
             

                track.IsDeleted = true;
                track.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();

                return Result<string>.SuccessResult($"Track {id} has been soft deleted.","Track deleted successfully.");
              
            }
            catch (Exception ex)
            {
                return Result<string>.FailureResult("Error deleting track.", ex.Message);
               
            }
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
