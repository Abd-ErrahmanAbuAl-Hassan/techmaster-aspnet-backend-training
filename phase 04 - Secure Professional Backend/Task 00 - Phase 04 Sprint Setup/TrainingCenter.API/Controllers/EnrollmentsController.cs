using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Application.DTOs.Enrollment.Requests;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ITrainingTrackService _trackService;

        public EnrollmentsController(IEnrollmentService enrollmentService, ITrainingTrackService trackService)
        {
            _enrollmentService = enrollmentService;
            _trackService = trackService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEnrollments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] EnrollmentStatus? status = null, [FromQuery] int? trackId = null, [FromQuery] int? studentId = null, [FromQuery] PaymentStatus? paymentStatus = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (trackId < 1 || studentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _enrollmentService.GetEnrollmentsAsync(pageNumber, pageSize, status, trackId, studentId, paymentStatus);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            if (!User.IsAdmin() && result.Data!.Student.Id != User.GetUserId())
                return this.ForbiddenResponse("You can only view your own enrollments.");

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            if (request.TrainingTrackId < 1 || request.StudentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (!User.IsAdmin())
            {
                if (request.StudentId != User.GetUserId())
                    return this.ForbiddenResponse("You can only enroll yourself.");

                request.AllowInactiveStudent = false;
            }

            var result = await _enrollmentService.CreateEnrollmentAsync(request);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return CreatedAtAction(nameof(GetEnrollmentById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEnrollmentStatus(int id, [FromBody] EnrollmentStatusUpdateRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                StatusCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _enrollmentService.UpdateEnrollmentStatusAsync(id, request.Status);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetStudentEnrollments(int studentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (studentId < 1) return BadRequest(new
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

            if (!User.IsAdmin() && User.GetUserId() != studentId)
                return this.ForbiddenResponse("You can only view your own enrollments.");

            var result = await _enrollmentService.GetStudentEnrollmentsAsync(studentId, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }

        [HttpGet("track/{trackId:int}/students")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetTrackStudents(int trackId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (trackId < 1) return BadRequest(new
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

            var (_, error) = await this.EnsureTrackAccessAsync(_trackService, trackId);
            if (error != null) return error;

            var result = await _enrollmentService.GetTrackStudentsAsync(trackId, pageNumber, pageSize);
            if (!result.Success) return this.FailureResponse(result.StatusCode, result);

            return Ok(result);
        }
    }
}