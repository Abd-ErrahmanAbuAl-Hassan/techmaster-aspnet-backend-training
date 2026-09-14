using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_01___EF_Core_Modeling_Drill_Pack.Data;
using Task_01___EF_Core_Modeling_Drill_Pack.DTOs;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public InstructorController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaginationResult<InstructorDTO>>>> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0)
            {
                return BadRequest("pageNumber must be greater than 0.");
            }

            if (pageSize < 1 || pageSize > 50)
            {
                return BadRequest("pageSize must be between 1 and 50.");
            }

            try
            {
                var totalCount = await _db.Instructors.CountAsync();

                var items = await _db.Instructors
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new InstructorDTO
                    {
                        Id = i.Id,
                        FName = i.FName,
                        LName = i.LName,
                        FullName = i.FullName,
                        Email = i.Email,
                        IsActive = i.IsActive,
                        CreatedAt = i.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new PaginationResult<InstructorDTO>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                });
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public async Task<ActionResult<InstructorDTO>> Create(CreateInstructorDTO createInstructorDTO)
        {
            try
            {
                if (createInstructorDTO is null)
                    return BadRequest("Instructor data is required.");

                if (string.IsNullOrWhiteSpace(createInstructorDTO.FName))
                    return BadRequest("First name is required.");

                if (string.IsNullOrWhiteSpace(createInstructorDTO.LName))
                    return BadRequest("Last name is required.");

                if (string.IsNullOrWhiteSpace(createInstructorDTO.Email))
                    return BadRequest("Email is required.");

                var emailExists = await _db.Instructors.AnyAsync(i => i.Email == createInstructorDTO.Email);
                if (emailExists)
                    return Conflict("An instructor with this email already exists.");

                var instructor = new Instructor
                {
                    FName = createInstructorDTO.FName,
                    LName = createInstructorDTO.LName,
                    Email = createInstructorDTO.Email,
                    IsActive = true
                };

                _db.Instructors.Add(instructor);
                await _db.SaveChangesAsync();

                var mappedInstructor = new InstructorDTO
                {
                    Id = instructor.Id,
                    FName = instructor.FName,
                    LName = instructor.LName,
                    FullName = instructor.FullName,
                    Email = instructor.Email,
                    IsActive = instructor.IsActive,
                    CreatedAt = instructor.CreatedAt
                };

                return CreatedAtAction(nameof(GetById), new { id = instructor.Id }, mappedInstructor);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InstructorDTO>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id must be a valid positive integer.");

                var instructor = await _db.Instructors.FindAsync(id);
                if (instructor is null)
                    return NotFound();

                var mappedInstructor = new InstructorDTO
                {
                    Id = instructor.Id,
                    FName = instructor.FName,
                    LName = instructor.LName,
                    FullName = instructor.FullName,
                    Email = instructor.Email,
                    IsActive = instructor.IsActive,
                    CreatedAt = instructor.CreatedAt
                };

                return Ok(mappedInstructor);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id:int}/tracks")]
        public async Task<ActionResult<IEnumerable<object>>> GetTracks(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id.");

            try
            {
                var instructor = await _db.Instructors
                    .Include(e => e.TrainingTracks)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (instructor is null) return NotFound();
                return Ok(instructor.TrainingTracks.Select(e => new
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description
                }));
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
