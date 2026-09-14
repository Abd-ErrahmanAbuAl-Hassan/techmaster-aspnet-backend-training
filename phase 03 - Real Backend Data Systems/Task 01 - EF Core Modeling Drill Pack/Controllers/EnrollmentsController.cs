using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_01___EF_Core_Modeling_Drill_Pack.Data;
using Task_01___EF_Core_Modeling_Drill_Pack.DTOs;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EnrollmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDTO>> Create(CreateEnrollmentDTO createEnrollmentDTO)
        {
            try
            {
                if (createEnrollmentDTO is null)
                    return BadRequest("Enrollment data is required.");

                if (createEnrollmentDTO.StudentId <= 0 || createEnrollmentDTO.TrainingTrackId <= 0)
                    return BadRequest("StudentId and TrainingTrackId must be valid positive integers.");

                var studentExists = await _db.Students.AnyAsync(s => s.Id == createEnrollmentDTO.StudentId);
                if (!studentExists)
                    return BadRequest("StudentId does not exist.");

                var trackExists = await _db.TrainingTracks.AnyAsync(t => t.Id == createEnrollmentDTO.TrainingTrackId);
                if (!trackExists)
                    return BadRequest("TrainingTrackId does not exist.");

                var duplicateActive = await _db.Enrollments.AnyAsync(e =>
                    e.StudentId == createEnrollmentDTO.StudentId &&
                    e.TrainingTrackId == createEnrollmentDTO.TrainingTrackId &&
                    e.Status == EnrollmentStatus.Active);
                if (duplicateActive)
                    return Conflict("Student already has an active enrollment in this track.");

                var enrollment = new Enrollment
                {
                    StudentId = createEnrollmentDTO.StudentId,
                    TrainingTrackId = createEnrollmentDTO.TrainingTrackId,
                    Status = createEnrollmentDTO.Status,
                    EnrollmentDate = createEnrollmentDTO.EnrollmentDate,
                    FinalGrade = createEnrollmentDTO.FinalGrade
                };

                _db.Enrollments.Add(enrollment);
                await _db.SaveChangesAsync();

                var mappedEnrollment = new EnrollmentDTO
                {
                    StudentName = enrollment.Student.FullName,
                    Track = enrollment.TrainingTrack.Name,
                    Status = enrollment.Status,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    FinalGrade = enrollment.FinalGrade
                };

                return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, mappedEnrollment);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EnrollmentDTO>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id must be a valid positive integer.");

                var enrollment = await _db.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (enrollment is null)
                    return NotFound();

                var mappedEnrollment = new EnrollmentDTO
                {
                    StudentName = enrollment.Student.FullName,
                    Track = enrollment.TrainingTrack.Name,
                    Status = enrollment.Status,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    FinalGrade = enrollment.FinalGrade
                };

                return Ok(mappedEnrollment);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("{id:int}/payment-summary")]
        public async Task<ActionResult<CreatePaymentSummaryDTO>> CreatePaymentSummary(int id, CreatePaymentSummaryDTO createPaymentSummaryDTO)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id must be a valid positive integer.");

                if (createPaymentSummaryDTO is null)
                    return BadRequest("Payment summary data is required.");

                if (createPaymentSummaryDTO.TotalRequired <= 0)
                    return BadRequest("TotalRequired must be a positive value.");

                if (createPaymentSummaryDTO.TotalPaid < 0)
                    return BadRequest("TotalPaid cannot be negative.");

                var enrollment = await _db.Enrollments.FindAsync(id);
                if (enrollment is null)
                    return NotFound();

                if (enrollment.PaymentSummary is not null)
                    return Conflict("This enrollment already has a payment summary.");

                var paymentSummary = new PaymentSummary
                {
                    EnrollmentId = id,
                    TotalRequired = createPaymentSummaryDTO.TotalRequired,
                    TotalPaid = createPaymentSummaryDTO.TotalPaid,
                    PaymentStatus = createPaymentSummaryDTO.PaymentStatus
                };

                _db.PaymentSummaries.Add(paymentSummary);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id }, createPaymentSummaryDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
