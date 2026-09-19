using Microsoft.AspNetCore.Mvc;
using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.Services.Interfaces;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] EnrollmentStatus? status = null, [FromQuery] int? trackId = null, [FromQuery] int? studentId = null, [FromQuery] PaymentStatus? paymentStatus = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            if (trackId < 1 || studentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _enrollmentService.GetEnrollmentsAsync(pageNumber, pageSize, status, trackId, studentId, paymentStatus);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            if (request.TrainingTrackId < 1 || request.StudentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _enrollmentService.CreateEnrollmentAsync(request);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateEnrollmentStatus(int id, [FromBody] EnrollmentStatusUpdateRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _enrollmentService.UpdateEnrollmentStatusAsync(id, request.Status);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentEnrollments(int studentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (studentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _enrollmentService.GetStudentEnrollmentsAsync(studentId, pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpGet("track/{trackId}/students")]
        public async Task<IActionResult> GetTrackStudents(int trackId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (trackId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });
            var result = await _enrollmentService.GetTrackStudentsAsync(trackId, pageNumber, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}