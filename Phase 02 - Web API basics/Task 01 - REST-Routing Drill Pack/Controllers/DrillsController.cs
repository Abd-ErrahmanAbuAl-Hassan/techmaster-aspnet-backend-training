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
        public IActionResult CelsiusToFahrenheit([FromQuery] decimal celsius )
        {
            try
            {
                var fahrenheit = _converterService.ConvertCelsiusToFahrenheit(celsius);

                return Ok(new { success = true, result = $"Fahrenheit = {fahrenheit}" });
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,new { success = false, message = $"UnExpected Error. Try again." });
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
            return Created(new Uri($"https://localhost:7241/api/Drills/notes/{note.Id}") , note);

        }
        [HttpGet("notes")]
        public IActionResult GetNotes()
        {
           if(_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

            return Ok(new {success = true , data = _notes , message = $"Successful retrival ({_notes?.Count ?? 0}) of notes"});

        }

        [HttpGet("notes/{id}")]
        public IActionResult GetNoteById(Guid id)
        {
           if(_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

           var note = _notes.FirstOrDefault(n=>n.Id == id);
           
            if(note is null)
                return NotFound(new { success = false, data = new List<Note>(), message = $"Note not found with id:{id}" });

            return Ok(new {success = true , data = note , message = $"Successful retrival."});

        }

        [HttpPut("notes/{id}")]
        public IActionResult UpdateNote(Guid id ,UpdateNoteRequest model)
        {
           if(_notes is null || !_notes.Any())
                return NotFound(new { success = false, data = new List<Note>(), message = $"There is no notes yet, Create your first note." });

           var note = _notes.FirstOrDefault(n=>n.Id == id);
           
            if(note is null)
                return NotFound(new { success = false, data = new List<Note>(), message = $"Note not found with id:{id}" });

            var noteIndex = _notes.IndexOf(note);

            note.Title = model.Title;
            note.Content = model.Content;

            _notes[noteIndex] = note;


            return Ok(new {success = true , data = note , message = $"Successful update note with id:{id}"});

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
    }
}
