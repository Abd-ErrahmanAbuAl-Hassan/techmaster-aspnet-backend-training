using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Application.DTOs.User.Requests;
using TrainingCenter.Application.Services.Interfaces;

namespace TrainingCenter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class InstructorsController : ControllerBase
    {
        private readonly IInstructorService _instructorService;

        public InstructorsController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInstructors([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _instructorService.GetInstructorsAsync(pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("my-tracks")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetMyTracks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _instructorService.GetInstructorTracksAsync(userId.Value, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetInstructorById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (!User.IsAdmin() && User.GetUserId() != id)
                return this.ForbiddenResponse("You can only view your own instructor profile.");

            var result = await _instructorService.GetInstructorByIdAsync(id);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("{id:int}/tracks")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetInstructorTracks(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
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

            if (!User.IsAdmin() && User.GetUserId() != id)
                return this.ForbiddenResponse("You can only view your own tracks.");

            var result = await _instructorService.GetInstructorTracksAsync(id, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
        {
            if (request == null) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Request body is required." }
            });

            var result = await _instructorService.CreateInstructorAsync(request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return CreatedAtAction(nameof(GetInstructorById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateInstructor(int id, [FromBody] UpdateInstructorRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (request == null) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Request body is required." }
            });

            if (!User.IsAdmin() && User.GetUserId() != id)
                return this.ForbiddenResponse("You can only update your own instructor profile.");

            if (!User.IsAdmin())
                request.IsActive = null;

            var result = await _instructorService.UpdateInstructorAsync(id, request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }
    }
}