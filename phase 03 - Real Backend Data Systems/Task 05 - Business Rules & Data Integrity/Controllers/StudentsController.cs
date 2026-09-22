using Microsoft.AspNetCore.Mvc;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;

namespace Task_05_Business_Rules_Data_Integrity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isActive = null,[FromQuery] bool? isDeleted = null)
        {
            if (pageNumber < 1 || pageSize < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "Page and page size must be positive." }
            });

            var result = await _studentService.GetStudentsAsync(pageNumber, pageSize, search, isActive,isDeleted);

            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _studentService.GetStudentByIdAsync(id);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
        {
            var result = await _studentService.CreateStudentAsync(request);
            if (!result.Success)
            {
                if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return CreatedAtAction(nameof(GetStudentById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });

            var result = await _studentService.UpdateStudentAsync(id, request);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 400) return StatusCode(StatusCodes.Status400BadRequest, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (id < 1) return BadRequest(new
            {
                Success = false,
                Message = "Validation error",
                ErrorCode = 400,
                Errors = new List<string> { "ID must be a positive number." }
            });
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result.Success)
            {
                if (result.ErrorCode == 404) return StatusCode(StatusCodes.Status404NotFound, result);
                else if (result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}