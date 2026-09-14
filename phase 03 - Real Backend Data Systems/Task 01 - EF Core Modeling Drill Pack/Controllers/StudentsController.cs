using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Task_01___EF_Core_Modeling_Drill_Pack.Data;
using Task_01___EF_Core_Modeling_Drill_Pack.DTOs;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public StudentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResult<StudentListItemDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
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
                var totalCount = await _db.Students.CountAsync();

                var items = await _db.Students
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new StudentListItemDto
                    {
                        Id = s.Id,
                        FullName = s.FullName,
                        Email = s.Email,
                        IsActive = s.IsActive
                    })
                    .ToListAsync();

                return Ok(new PaginationResult<StudentListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                });
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Student>> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id");
            try
            {
                var student = await _db.Students.FindAsync(id);
                if (student is null) return NotFound();

                var mappedStudent = new StudentListItemDto
                {

                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    IsActive = student.IsActive
                };
                return Ok(student);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Student>> Create([FromBody]CreateStudentRequest model)
        {
            if(model == null) return BadRequest(ModelState);
            try
            {
                var student = new Student
                {
                    FName = model.FName,
                    LName = model.LName,
                    IsActive = true,
                    Email = model.Email
                };

                var profile = new StudentProfile
                {
                    SSN = model.SSN,
                    Address = model.Address,
                    EmergencyPhone = model.EmergencyPhone,
                    BirthOfDate = model.BirthOfDate,
                    Student = student
                };
                _db.Students.Add(student);
                _db.StudentsProfile.Add(profile);

                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = student.Id }, new 
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    CreatedAt = student.CreatedAt,
                    IsActive = student.IsActive
                });
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException.Message ?? e.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateStudentRequest model)
        {
            if (id <= 0) return BadRequest("Invalid Id.");

            try
            {
                var student = await _db.Students.Include(s=>s.StudentProfile)
                                                .FirstOrDefaultAsync(s=>s.Id == id);

                if (student is null) return NotFound();

                bool change = false;

                if(!string.IsNullOrWhiteSpace(model.FName)) { student.FName = model.FName!; change = true; }
                if(!string.IsNullOrWhiteSpace(model.LName)) { student.LName = model.LName!; change = true; }
                if(!string.IsNullOrWhiteSpace(model.Email)) { student.Email = model.Email!; change = true; }
                if(model.IsActive.HasValue && model.IsActive != student.IsActive) { student.IsActive = (bool)model.IsActive; change = true; }
                if(!string.IsNullOrWhiteSpace(model.Address)) { student.StudentProfile.Address = model.Address!; change = true; }
                if(!string.IsNullOrWhiteSpace(model.SSN)) { student.StudentProfile.SSN = model.SSN!; change = true; }
                if(!string.IsNullOrWhiteSpace(model.EmergencyPhone)) { student.StudentProfile.EmergencyPhone = model.EmergencyPhone!; change = true; }
                if(model.BirthOfDate.HasValue) { student.StudentProfile.BirthOfDate = (DateTime)model.BirthOfDate!; change = true; }

                if (change)
                {
                    student.UpdatedAt = DateTime.UtcNow;
                    _db.Entry(student).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return NoContent();
                }

                return BadRequest("Update Model Request has invalid data.");
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id.");

            try
            {
                var student = await _db.Students.FindAsync(id);
                if (student is null) return NotFound();

                student.IsDeleted = true;
                student.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<PaginationResult<StudentListItemDto>>>> GetDeleted([FromQuery] int pageNumber = 1,
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
                var totalCount = await _db.Students.CountAsync();

                var items = await _db.Students
                    .IgnoreQueryFilters()
                    .Where(s => s.IsDeleted)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new StudentListItemDto
                    {
                        Id = s.Id,
                        FullName = s.FullName,
                        Email = s.Email,
                        IsActive = s.IsActive
                    })
                    .ToListAsync();

                return Ok(new PaginationResult<StudentListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                });
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id:int}/tracks")]
        public async Task<ActionResult<IEnumerable<TrainingTrack>>> GetTracks(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id.");

            try
            {
                var student = await _db.Students
                    .Include(s => s.Enrollments)
                    .ThenInclude(e => e.TrainingTrack)
                    .ThenInclude(t => t.Instructor)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student is null) return NotFound();
                return Ok(student.Enrollments.Select(e => new TrackDto
                {
                    Id = e.TrainingTrack.Id,
                    Name = e.TrainingTrack.Name,
                    Description = e.TrainingTrack.Description,
                    InstructorName = e.TrainingTrack.Instructor.FullName
                }));
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
