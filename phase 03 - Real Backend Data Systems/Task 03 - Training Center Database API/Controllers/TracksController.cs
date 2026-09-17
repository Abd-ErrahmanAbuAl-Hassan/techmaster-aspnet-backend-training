using Microsoft.AspNetCore.Mvc;
using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.Services.Interfaces;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ControllerBase
    {
        private readonly ITrainingTrackService _trackService;

        public TracksController(ITrainingTrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTracks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null, [FromQuery] TrackLevel? level = null, [FromQuery] TrackStatus? status = null, [FromQuery] int? instructorId = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (instructorId.HasValue && instructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Instructor ID must be positive." }
            });

            var result = await _trackService.GetTracksAsync(pageNumber, pageSize, keyword, level, status, instructorId);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrackById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _trackService.GetTrackByIdAsync(id);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
        {
            if (request.InstructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Instructor ID must be a positive number." }
            });

            var result = await _trackService.CreateTrackAsync(request);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            
            return CreatedAtAction(nameof(GetTrackById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrack(int id, [FromBody] UpdateTrackRequest request)
        {
            if (id < 1 || request.InstructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _trackService.UpdateTrackAsync(id, request);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 403) return StatusCode(StatusCodes.Status403Forbidden, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(int id , [FromQuery]int instructorId)
        {
            if (id < 1 || instructorId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _trackService.DeleteTrackAsync(id,instructorId);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 403) return StatusCode(StatusCodes.Status403Forbidden, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
    }
}