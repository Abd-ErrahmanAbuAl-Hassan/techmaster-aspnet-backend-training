using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services
{
    public class TrainingTrackService : ITrainingTrackService
    {
        private readonly ApplicationDbContext _context;

        public TrainingTrackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<TrackDetailsResponse>>> GetTracksAsync(int pageNumber = 1, int pageSize = 10, string? keyword = null, TrackLevel? level = null, TrackStatus? status = null, int? instructorId = null)
        {
            try
            {
                var query = _context.TrainingTracks
                    .Include(t => t.Instructor)
                    .Include(t => t.Enrollments)
                    .Where(t => !t.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(t => t.Title.Contains(keyword) || t.Description.Contains(keyword));
                if (level.HasValue)
                    query = query.Where(t => t.Level == level.Value);
                if (status.HasValue)
                    query = query.Where(t => t.Status == status.Value);
                if (instructorId.HasValue)
                    query = query.Where(t => t.InstructorId == instructorId.Value);

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
                        Message = "No tracks are found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                var response = tracks.Select(t => MapToTrackDetailsResponse(t)).ToList();

                return new ApiResponse<PagedResult<TrackDetailsResponse>>
                {
                    Success = true,
                    Message = "Tracks retrieved successfully.",
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
                    Message = "Error retrieving tracks.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<TrackDetailsResponse>> GetTrackByIdAsync(int id)
        {
            try
            {
                var track = await _context.TrainingTracks
                    .Include(t => t.Instructor)
                    .Include(t => t.Enrollments)
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (track == null)
                    return new ApiResponse<TrackDetailsResponse>
                    {
                        Success = false,
                        Message = "Track not found.",
                        ErrorCode = 404
                    };

                return new ApiResponse<TrackDetailsResponse>
                {
                    Success = true,
                    Message = "Track retrieved successfully.",
                    Data = MapToTrackDetailsResponse(track)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TrackDetailsResponse>
                {
                    Success = false,
                    Message = "Error retrieving track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<TrackDetailsResponse>> CreateTrackAsync(CreateTrackRequest request)
        {
            try
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Title))
                    errors.Add("Track name is required.");
                if (string.IsNullOrWhiteSpace(request.Description))
                    errors.Add("Track description is required.");
                if (request.Capacity <= 0)
                    errors.Add("Capacity must be greater than zero.");
                if (request.Price <= 0)
                    errors.Add("Price must be greater than zero.");
                if (request.StartDate >= request.EndDate)
                    errors.Add("StartDate must be before EndDate.");

                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == request.InstructorId);
                if (instructor == null)
                    errors.Add("Instructor not found.");

                if (errors.Any())
                    return new ApiResponse<TrackDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                string code;
                var attempts = 0;
                do
                {
                    code = $"{RandomNumberGenerator.GetInt32(0, 100000):D6}";
                    attempts++;
                }
                while (await _context.TrainingTracks.AnyAsync(t => t.Code == code) && attempts < 10);

                if (await _context.TrainingTracks.AnyAsync(t => t.Code == code))
                    return new ApiResponse<TrackDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Could not generate a unique track code. Please try again." }
                    };

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
                    //CreatedAt = DateTime.UtcNow
                };

                _context.TrainingTracks.Add(track);
                await _context.SaveChangesAsync();

                track = await _context.TrainingTracks
                    .Include(t => t.Instructor)
                    .Include(t => t.Enrollments)
                    .FirstAsync(t => t.Id == track.Id);

                return new ApiResponse<TrackDetailsResponse>
                {
                    Success = true,
                    Message = "Track created successfully.",
                    Data = MapToTrackDetailsResponse(track)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TrackDetailsResponse>
                {
                    Success = false,
                    Message = "Error creating track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<TrackDetailsResponse>> UpdateTrackAsync(int id, UpdateTrackRequest request)
        {
            try
            {
                var track = await _context.TrainingTracks
                    .Include(t => t.Instructor)
                    .Include(t => t.Enrollments)
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (track == null)
                    return new ApiResponse<TrackDetailsResponse>
                    {
                        Success = false,
                        Message = "Track not found.",
                        ErrorCode = 404
                    };

                if (track.InstructorId != request.InstructorId)
                    return new ApiResponse<TrackDetailsResponse>
                    {
                        Success = false,
                        Message = "Access Denied.",
                        ErrorCode = 403,
                        Errors = new List<string> { $"Instructor id:{request.InstructorId} not allowed to access." }
                    };

                var errors = new List<string>();

                if (request.Capacity.HasValue && request.Capacity <= 0)
                    errors.Add("Capacity must be greater than zero.");

                if (request.Price.HasValue && request.Price <= 0)
                    errors.Add("Price must be greater than zero.");

                var effectiveStart = request.StartDate ?? track.StartDate;
                var effectiveEnd = request.EndDate ?? track.EndDate;
                if (effectiveStart >= effectiveEnd)
                    errors.Add("StartDate must be before EndDate.");

                if (errors.Any())
                    return new ApiResponse<TrackDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

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

                //track.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ApiResponse<TrackDetailsResponse>
                {
                    Success = true,
                    Message = "Track updated successfully.",
                    Data = MapToTrackDetailsResponse(track)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TrackDetailsResponse>
                {
                    Success = false,
                    Message = "Error updating track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteTrackAsync(int id, int InstructorId)
        {
            try
            {
                var track = await _context.TrainingTracks
                    .Include(t => t.Enrollments)
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (track == null)
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Track not found.",
                        ErrorCode = 404
                    };

                if (track.InstructorId != InstructorId)
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Access Denied.",
                        ErrorCode = 403,
                        Errors = new List<string> { $"Instructor id:{InstructorId} not allowed to access." }
                    };

                var activeEnrollments = track.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
                if (activeEnrollments > 0)
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Cannot delete track with active enrollments.",
                        ErrorCode = 400,
                        Errors = new List<string> { $"Track has {activeEnrollments} active enrollment(s)." }
                    };

                track.IsDeleted = true;
                track.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Track deleted successfully.",
                    Data = $"Track {id} has been soft deleted."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error deleting track.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private TrackDetailsResponse MapToTrackDetailsResponse(TrainingTrack track)
        {
            var enrolledCount = track.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
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
