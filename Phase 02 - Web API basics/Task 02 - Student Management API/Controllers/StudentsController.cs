using Microsoft.AspNetCore.Mvc;
using Task_02___Student_Management_API.DTOs;
using Task_02___Student_Management_API.Services;
using Task_02___Student_Management_API.Utilities;

namespace Task_02___Student_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("create")]
        public IActionResult CreateStudent(CreateStudentRequest model)
        {
            if (model is null) return BadRequest(new
            {
                success = false,
                message = "Creation model is required.",
                error = "creation model is null.".ToList()
            });

            var result = _studentService.Create(model);

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);

            return Created($"/api/students/{result.Data.Id}", result);
        }

        [HttpGet("all")]
        public IActionResult GetAllStudent([FromQuery] Filter? filter)
        {
            var result = _studentService.GetAll(filter);

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);

            return Ok(result);
        }

        [HttpGet("stats")]
        public IActionResult GetStudentStats()
        {
            var result = _studentService.GetStats();

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);

            return Ok(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetStudentById(Guid id)
        {
            var result = _studentService.GetById(id);

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(Guid id, UpdateStudentRequest model)
        {
            var result = _studentService.Update(id, model);

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);

            return Ok(result);
        }
        [HttpPatch("{id}/status")]
        public IActionResult UpdateStudentStatus(Guid id, [FromQuery] UpdateStudentStatusRequest model)
        {
            var result = _studentService.Update(id, model);

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);
            if (!result.Success && result.ErrorCode == 409) return Conflict(result);

            return Ok(result);
        }
        [HttpDelete("{id}/delete")]
        public IActionResult DeleteStudent(Guid id)
        {
            var result = _studentService.Delete(id);

            if (!result.Success && result.ErrorCode == 400) return BadRequest(result);
            if (!result.Success && result.ErrorCode == 404) return NotFound(result);
            if (!result.Success && result.ErrorCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(result);
        }
    }
}
