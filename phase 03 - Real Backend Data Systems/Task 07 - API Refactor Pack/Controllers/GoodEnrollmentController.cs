using Microsoft.AspNetCore.Mvc;
using Task_07___API_Refactor_Pack.DTOs;
using Task_07___API_Refactor_Pack.Services;

namespace Task_07___API_Refactor_Pack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodEnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public GoodEnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery]int page =1 , [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _enrollmentService.GetEnrollmentsAsync(page, pageSize);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateEnrollmentRequest request)
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
        [HttpGet("{id}")]
        private object GetEnrollmentById(int enrollmentId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("pay")]
        public async Task<ActionResult> Pay([FromBody] CreatePaymentRequest request)
        {
            if (request.EnrollmentId < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _enrollmentService.CreatePaymentAsync(request);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 409) return StatusCode(StatusCodes.Status409Conflict, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be positive." }
            });

            var result = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}
