using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Application.DTOs.User.Requests;
using TrainingCenter.Application.Services.Interfaces;

namespace TrainingCenter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;

        public StudentsController(IStudentService studentService, IEnrollmentService enrollmentService)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isActive = null, [FromQuery] bool? isDeleted = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _studentService.GetStudentsAsync(pageNumber, pageSize, search, isActive, isDeleted);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _studentService.GetStudentByIdAsync(userId.Value);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("my-enrollments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEnrollments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
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

            var result = await _enrollmentService.GetStudentEnrollmentsAsync(userId.Value, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (!User.IsAdmin() && User.GetUserId() != id)
                return this.ForbiddenResponse("You can only view your own profile.");

            var result = await _studentService.GetStudentByIdAsync(id);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
        {
            var result = await _studentService.CreateStudentAsync(request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return CreatedAtAction(nameof(GetStudentById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (!User.IsAdmin() && User.GetUserId() != id)
                return this.ForbiddenResponse("You can only update your own profile.");

            if (!User.IsAdmin())
                request.IsActive = null;

            var result = await _studentService.UpdateStudentAsync(id, request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (!User.IsAdmin() && User.GetUserId() != id)
                return this.ForbiddenResponse("You can only delete your own account.");

            var result = await _studentService.DeleteStudentAsync(id);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }
    }
}