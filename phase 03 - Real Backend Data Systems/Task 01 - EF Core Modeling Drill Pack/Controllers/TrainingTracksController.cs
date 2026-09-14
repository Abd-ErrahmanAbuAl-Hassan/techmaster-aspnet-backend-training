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
    [Route("api/tracks")]
    public class TrainingTracksController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public TrainingTracksController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaginationResult<TrackDetailsDto>>>> GetAll([FromQuery] int pageNumber = 1,
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
                var totalCount = await _db.TrainingTracks
                    .IgnoreQueryFilters()
                    .Where(t => !t.IsDeleted).CountAsync();

                var items = await _db.TrainingTracks
                    .IgnoreQueryFilters()
                    .Where(t=>!t.IsDeleted)
                    .Include(t=>t.Enrollments)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new TrackDetailsDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        InstructorName = e.Instructor.FullName,
                        EnrolledStudentCount = e.Enrollments.Count(),
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(new PaginationResult<TrackDetailsDto>
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

        [HttpPost]
        public async Task<ActionResult<TrainingTrack>> Create([FromQuery]int instructorId , [FromBody]CreateTrackRequest model)
        {
            if (instructorId <= 0) return BadRequest("Invalid Id.");
            if (model == null) return BadRequest(ModelState);

            try
            {
                var instructorExists = await _db.Instructors.AnyAsync(i => i.Id == instructorId);
                if (!instructorExists)
                {
                    return BadRequest($"Instructor {instructorId} does not exist.");
                }

                var track = new TrainingTrack
                {
                    Name = model.Name,
                    Description = model.Description,
                    InstructorId = instructorId,
                };

                _db.TrainingTracks.Add(track);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = track.Id }, new
                {
                    Id = track.Id,
                    Name = track.Name,
                    Description = track.Description,
                    InstructorId = instructorId,
                    CreatedAt = track.CreatedAt
                });
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException.Message ?? e.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateTrackRequest model)
        {
            if (id <= 0 || model.InstructorId <= 0) return BadRequest("Invalid Id.");
            if (model == null) return BadRequest(ModelState);

            try
            {
                var track = await _db.TrainingTracks.Include(t=>t.Instructor).FirstOrDefaultAsync(s => s.Id == id);

                if (track is null) return NotFound();

                if (track.Instructor.Id != model.InstructorId)
                {
                    return Forbid($"Instructor {model.InstructorId} unauthorized.");
                }

                bool change = false;

                if (!string.IsNullOrWhiteSpace(model.Name)) { track.Name = model.Name!; change = true; }
                if (!string.IsNullOrWhiteSpace(model.Description)) { track.Description = model.Description!; change = true; }

                if (change)
                {
                    await _db.SaveChangesAsync();
                    return NoContent();
                }

                return BadRequest("Update Model Request has invalid data.");
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException.Message ?? e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TrackDetailsDto>> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id.");

            try
            {
                var track = await _db.TrainingTracks
                        .Where(t => t.Id == id)
                        .Select(t => new TrackDetailsDto
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Description = t.Description,
                            InstructorName = t.Instructor.FullName,
                            EnrolledStudentCount = t.Enrollments.Count,
                            CreatedAt = t.CreatedAt,
                            UpdatedAt = t.UpdatedAt
                        })
                        .FirstOrDefaultAsync();

                if (track is null) return NotFound();
                return Ok(track);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id:int}/students")]
        public async Task<ActionResult<IEnumerable<TrackStudents>>> GetStudents(int id, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (id <= 0) return BadRequest("Invalid Id.");
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
                var track = await _db.TrainingTracks
                       .Include(t => t.Enrollments)
                       .ThenInclude(e => e.Student)
                       .FirstOrDefaultAsync(t => t.Id == id);

                if (track is null) return NotFound();
                var students = track.Enrollments
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .Select(e => new StudentListItemDto()
                               {
                                   Id = e.Student.Id,
                                   FullName = e.Student.FullName,
                                   IsActive = e.Student.IsActive,
                                   Email = e.Student.Email

                               }).ToList();

                var trackStudents = new TrackStudents
                {
                    TrackId= track.Id,
                    TrackName = track.Name,
                    Students = new PaginationResult<StudentListItemDto>
                    {
                        Items = students,
                        TotalCount = track.Enrollments.Count(),
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                               
                };
                return Ok(trackStudents);
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
                var track = await _db.TrainingTracks.FindAsync(id);
                if (track is null) return NotFound();

                track.IsDeleted = true;
                track.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
