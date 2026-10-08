using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using TrainingCenter.Application.DTOs.Track.Requests;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TracksController : ControllerBase
    {
        private readonly ITrainingTrackService _trackService;
        private readonly IEnrollmentService _enrollmentService;

        public TracksController(ITrainingTrackService trackService, IEnrollmentService enrollmentService)
        {
            _trackService = trackService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTracks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null, [FromQuery] TrackLevel? level = null, [FromQuery] TrackStatus? status = null, [FromQuery] int? instructorId = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (instructorId.HasValue && instructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Instructor ID must be positive." }
            });

            var result = await _trackService.GetTracksAsync(pageNumber, pageSize, keyword, level, status, instructorId);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("available")]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> GetAvailableTracks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null, [FromQuery] TrackLevel? level = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _trackService.GetTracksAsync(pageNumber, pageSize, keyword, level, TrackStatus.Published, null);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> GetTrackById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _trackService.GetTrackByIdAsync(id);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            var track = result.Data!;

            if (User.IsInRole("Instructor") && track.Instructor.Id != User.GetUserId())
                return this.ForbiddenResponse("You can only view tracks assigned to you.");

            if (User.IsInRole("Student") && track.Status != TrackStatus.Published)
                return NotFound(new
                {
                    Success = false,
                    Message = "Track not found.",
                    StatusCode = 404,
                    Errors = new List<string> { "Track not found." }
                });

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
        {
            if (request.InstructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Instructor ID must be a positive number." }
            });

            var userId = User.GetUserId();
            if (userId == null) return this.FailureResponse(401, new { message = "Unauthorized" });

            var result = await _trackService.CreateTrackAsync(request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return CreatedAtAction(nameof(GetTrackById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateTrack(int id, [FromBody] UpdateTrackRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var (track, error) = await this.EnsureTrackAccessAsync(_trackService, id);
            if (error != null) return error;

            request.InstructorId = track!.Instructor.Id;

            var result = await _trackService.UpdateTrackAsync(id, request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var (track, error) = await this.EnsureTrackAccessAsync(_trackService, id);
            if (error != null) return error;

            var result = await _trackService.DeleteTrackAsync(id, track!.Instructor.Id);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("{id:int}/students")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetTrackStudents(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var (_, error) = await this.EnsureTrackAccessAsync(_trackService, id);
            if (error != null) return error;

            var result = await _enrollmentService.GetTrackStudentsAsync(id, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpPut("{trackId:int}/assign-instructor")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignInstructorToTrack(int trackId, [FromBody] int instructorId)
        {
            if (trackId < 1 || instructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (User.GetUserId() == null) return this.FailureResponse(401, new { message = "Unauthorized" });
            var result = await _trackService.AssignInstructorToTrackAsync(trackId, instructorId);

            if (!result.Success) this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpPost("~/api/instructor/tracks/{trackId:int}/sessions")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> CreateTrackSession(int trackId, [FromBody] CreateTrackSessionRequest request)
        {

            var (_, error) = await this.EnsureTrackAccessAsync(_trackService, trackId);
            if (error != null) return error;

            var result = await _trackService.CreateTrackSessionAsync(trackId, request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Created("~/api/track/session/{id}", result); // get by id endpoint not implemented yet.
        }

        [HttpPut("~/api/instructor/sessions/{sessionId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> CreateTrackSession(int sessionId, [FromBody] UpdateSessionRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (userId != request.InstructorId && !User.IsAdmin()) this.ForbiddenResponse("You can only access own sessions.");

            var result = await _trackService.UpdateTrackSessionAsync(sessionId, request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result); 
        }

        [HttpGet("~/api/instructor/tracks/{trackId:int}/progress")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> CreateTrackSession(int trackId, [FromQuery] int instructorId)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (userId != instructorId && !User.IsAdmin()) this.ForbiddenResponse("You can only access own sessions.");

            var result = await _trackService.GetTrackProgressAsync(trackId, instructorId);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result); 
        }
    }
}