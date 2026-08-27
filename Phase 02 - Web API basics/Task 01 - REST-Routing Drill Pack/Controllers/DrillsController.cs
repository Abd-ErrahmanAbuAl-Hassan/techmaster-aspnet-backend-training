using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_01___REST_Routing_Drill_Pack.DTOs;
using Task_01___REST_Routing_Drill_Pack.Entities;
using Task_01_REST_Routing_Drill_Pack.Services;

namespace Task_01___REST_Routing_Drill_Pack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrillsController : ControllerBase
    {
        private readonly ConverterService _converterService;
        private static List<Note> _notes = new();
        public DrillsController(ConverterService converterService)
        {
            _converterService = converterService;
        }
        [HttpGet("health")]
        public IActionResult Index()
        {
            return Ok(new { status = "Running", service = "TechMaster API", time = DateTime.UtcNow });
        }

        [HttpGet("tools/echo/{name}")]
        public IActionResult Echo([FromRoute] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { success = false, message = "Name is required." });

            return Ok(new { success = true, message = $"Hi, {name.Trim()}" });


        }

        [HttpGet("calculator/add")]
        public IActionResult Add([FromQuery] decimal a, [FromQuery] decimal b)
        {
            if (b < 0) return Ok(new { success = true, result = $"{a} - {b * -1} = {a + b}" });

            return Ok(new { success = true, result = $"{a} + {b} = {a + b}" });
        }

        [HttpGet("converter/celsius-to-fahrenheit")]
        public IActionResult CelsiusToFahrenheit([FromQuery] decimal celsius)
        {
            try
            {
                var fahrenheit = _converterService.ConvertCelsiusToFahrenheit(celsius);

                return Ok(new { success = true, result = $"Fahrenheit = {fahrenheit}" });
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = $"UnExpected Error. Try again." });
            }
        }

        [HttpGet("grades/calculate")]
        public IActionResult GradeCalculator([FromQuery] decimal score)
        {

            if (score < 0 || score > 100) return BadRequest(new { success = false, message = $"Score must be between 0 and 100" });

            char grade;

            if (score > 89) grade = 'A';
            else if (score > 79) grade = 'B';
            else if (score > 69) grade = 'C';
            else if (score > 59) grade = 'D';
            else grade = 'F';

            return Ok(new { success = true, result = $"Grade = {grade}" });

        }

        [HttpPost("notes")]
        public IActionResult CreateNote([FromBody] CreateNoteRequest model)
        {
            if (model is null) return BadRequest(new { success = false, message = "Note Title and content is required." });

            var note = new Note
            {
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.Now
            };

            _notes.Add(note);
            return Created($"/api/Drills/notes/{note.Id}", note);

        }

        [HttpGet("notes")]
        public IActionResult GetNotes()
        {
            if (_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            return Ok(new { success = true, data = _notes, message = $"Successful retrival ({_notes?.Count ?? 0}) of notes" });

        }
        [HttpGet("notes/pagination")]
        public IActionResult GetNotesByPagination([FromQuery]int page = 1, [FromQuery]int pageSize = 5)
        {
            if (page < 1 || pageSize < 1) return BadRequest(new { success = false, data = new List<Note>(), message = $"Page and page size must be greater than 0." });

            if (pageSize > 50) return BadRequest(new { success = false, data = new List<Note>(), message = $"page size must be less than 50." });

            if (_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            var notes = _notes.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(new { success = true, data = notes, message = $"Successful retrival ({notes?.Count ?? 0}) of notes, total-count = {_notes?.Count ?? 0}" });

        }

        [HttpGet("notes/{id}")]
        public IActionResult GetNoteById(Guid id)
        {
            if (_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            var note = _notes.FirstOrDefault(n => n.Id == id);

            if (note is null)
                return NotFound(new { success = false, data = new List<Note>(), message = $"Note not found with id:{id}" });

            return Ok(new { success = true, data = note, message = $"Successful retrival." });

        }

        [HttpPut("notes/{id}")]
        public IActionResult UpdateNote(Guid id, UpdateNoteRequest model)
        {
            if (_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            var note = _notes.FirstOrDefault(n => n.Id == id);

            if (note is null)
                return NotFound(new { success = false, data = new List<Note>(), message = $"Note not found with id:{id}" });

            var noteIndex = _notes.IndexOf(note);

            note.Title = model.Title;
            note.Content = model.Content;

            _notes[noteIndex] = note;


            return Ok(new { success = true, data = note, message = $"Successful update note with id:{id}" });

        }

        [HttpDelete("notes/{id}")]
        public IActionResult DeleteNote(Guid id)
        {
            if (_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            var note = _notes.FirstOrDefault(n => n.Id == id);

            if (note is null)
                return NotFound(new { success = false, data = new List<Note>(), message = $"Note not found with id:{id}" });

            _notes.Remove(note);

            return NoContent();

        }

        [HttpGet("notes/search")]
        public IActionResult SearchNotes([FromQuery] string searchTerm)
        {
            if (_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            var notes = _notes.Where(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                     || n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (notes is null)
                return NotFound(new { success = false, data = new List<Note>(), message = $"No notes found" });

            return Ok(new { success = true, data = notes, message = $"Successful retrival ({notes?.Count ?? 0}) of notes" });


        }

        [HttpGet("request-info")]
        public IActionResult RequestInfo()
        {
            var studentName = Request.Headers["X-Student-Name"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(studentName))
                return BadRequest(new { success = false, message = "X-Student-Name header is required." });

            return Ok(new
            {
                success = true,
                studentName = studentName,
                requestPath = Request.Path,
                message = "Header successfully read."
            });
        }

        [HttpGet("status-codes/success")]
        public IActionResult StatusCodeSuccess()
        {
            return Ok(new { success = true, message = "200 OK - Request succeeded.", statusCode = 200 });
        }

        [HttpPost("status-codes/created")]
        public IActionResult StatusCodeCreated()
        {
            var newResource = new { id = Guid.NewGuid(), name = "New Resource" };
            return Created($"/api/drills/status-codes/created/{newResource.id}",
                new { success = true, data = newResource, message = "201 Created - Resource created successfully.", statusCode = 201 });
        }

        [HttpDelete("status-codes/no-content")]
        public IActionResult StatusCodeNoContent()
        {
            return NoContent();
        }

        [HttpGet("status-codes/bad-request")]
        public IActionResult StatusCodeBadRequest()
        {
            return BadRequest(new { success = false, message = "400 Bad Request - Invalid input provided.", statusCode = 400 });
        }

        [HttpGet("status-codes/not-found")]
        public IActionResult StatusCodeNotFound()
        {
            return NotFound(new { success = false, message = "404 Not Found - Resource does not exist.", statusCode = 404 });
        }

        [HttpGet("errors/demo")]
        public IActionResult ErrorDemo([FromQuery] string errorType = "bad-request")
        {
            return errorType switch
            {
                "bad-request" => BadRequest(new
                {
                    success = false,
                    message = "Invalid request",
                    code = StatusCodes.Status400BadRequest,
                    errors = new[] { "Name is required", "Email format is invalid" }
                }),
                "not-found" => NotFound(new
                {
                    success = false,
                    message = "Resource not found",
                    code = StatusCodes.Status404NotFound,
                    errors = new[] { "User with id 123 does not exist" }
                }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Internal Server Error",
                    code = StatusCodes.Status500InternalServerError,
                    errors = new[] { "Internal Server Error" }
                })
            };
        }
    }
}
