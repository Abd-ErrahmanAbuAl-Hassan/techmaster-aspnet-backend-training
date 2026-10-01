using Task_07___API_Refactor_Pack.Entities;
using Task_07___API_Refactor_Pack.Data;
using Task_07___API_Refactor_Pack.DTOs;
using Task_07___API_Refactor_Pack.Entities;
using Task_07___API_Refactor_Pack.Utilities;
using Task_07___API_Refactor_Pack.Utilities.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace Task_07___API_Refactor_Pack.Services
{
    public class EnrollmentService:IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {

            try
            {
                var errors = new List<string>();
                if (request.StudentId < 1) errors.Add("Student ID must be positive number.");
                if (request.TrainingTrackId < 1) errors.Add("Track ID must be positive number.");

                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
                if (student == null)
                    errors.Add("Student not found.");

                var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.Id == request.TrainingTrackId
                                                                                && (t.Status == TrackStatus.Published
                                                                                || t.Status == TrackStatus.Closed));
                if (track == null)
                    errors.Add("Track not found.");

                if (errors.Any())
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                if (track!.Status == TrackStatus.Closed)
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "This track is closed and cannot accept new enrollments." }
                    };

                if (await _context.Enrollments.AnyAsync(e => e.StudentId == request.StudentId && e.TrainingTrackId == request.TrainingTrackId && e.Status != EnrollmentStatus.Cancelled))
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Student is already enrolled in this track." }
                    };

                var enrolledCount = await _context.Enrollments
                    .Where(e => e.TrainingTrackId == request.TrainingTrackId
                             && e.Status == EnrollmentStatus.Completed
                             || e.Status == EnrollmentStatus.Active)
                    .CountAsync();

                if (enrolledCount >= track!.Capacity)
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Track capacity is full." }
                    };

                var enrollment = new Enrollment
                {
                    StudentId = request.StudentId,
                    TrainingTrackId = request.TrainingTrackId,
                    Status = EnrollmentStatus.Draft,
                    EnrollmentDate = DateTime.UtcNow
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                enrollment = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .Include(e => e.Payments)
                    .FirstAsync(e => e.Id == enrollment.Id);

                return new ApiResponse<EnrollmentDetailsResponse>
                {
                    Success = true,
                    Message = "Enrollment created successfully.",
                    Data = MapToEnrollmentDetailsResponse(enrollment)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EnrollmentDetailsResponse>
                {
                    Success = false,
                    Message = "Error creating enrollment.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var errors = new List<string>();
                if (request.EnrollmentId < 1) errors.Add("Enrollment ID must be positive number.");
                if (request.Amount <= 0) errors.Add("Amount must be greater than zero.");
                if (!Enum.IsDefined(request.PaymentMethod)) errors.Add("Payment method not valid.");

                var enrollment = await _context.Enrollments
                                .Include(e => e.TrainingTrack)
                                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId);

                if (!(request.EnrollmentId < 1) && enrollment == null)
                    errors.Add("Enrollment not found.");

                if (errors.Any())
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                if (enrollment.Status == EnrollmentStatus.Cancelled)
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Cannot create a payment for a cancelled enrollment." }
                    };

                var totalPaid = await _context.Payments
                        .Where(p => p.EnrollmentId == request.EnrollmentId &&
                              (p.PaymentStatus == PaymentStatus.PartiallyPaid ||
                               p.PaymentStatus == PaymentStatus.Paid))
                        .SumAsync(p => p.Amount);

                decimal totalAmount = totalPaid + request.Amount;

                if (totalPaid >= enrollment.TrainingTrack!.Price)
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Conflict error",
                        ErrorCode = 409,
                        Errors = new List<string> { "The enrollment is already paid." }
                    };

                PaymentStatus status = PaymentStatus.Pending;

                if (totalAmount > enrollment!.TrainingTrack!.Price)
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Payment amount exceeds track price." }
                    };
                else if (totalAmount < enrollment!.TrainingTrack!.Price)
                    status = PaymentStatus.PartiallyPaid;
                else
                    status = PaymentStatus.Paid;

                var payment = new Payment
                {
                    EnrollmentId = request.EnrollmentId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = status,
                    ReferenceNumber = $"REF-{enrollment.Id}-{Guid.NewGuid():N}"[..20],
                    Notes = request.Notes
                };

                _context.Payments.Add(payment);

                if (totalAmount >= enrollment.TrainingTrack!.Price)
                {
                    enrollment.Status = EnrollmentStatus.Active;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse<PaymentResponse>
                {
                    Success = true,
                    Message = "Payment created successfully.",
                    Data = new PaymentResponse
                    {
                        Id = payment.Id,
                        Amount = payment.Amount,
                        PaymentMethod = payment.PaymentMethod,
                        PaymentDate = payment.PaymentDate,
                        PaymentStatus = payment.PaymentStatus,
                        ReferenceNumber = payment.ReferenceNumber
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<PaymentResponse>
                {
                    Success = false,
                    Message = "Error creating payment.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteEnrollmentAsync(int enrollmentId)
        {
            try
            {
                if(enrollmentId < 1) return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Validation Errors.",
                    ErrorCode = 400,
                    Errors = new List<string>() { "Enrollment ID must be a positive number"}
                };
                var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e=>e.Id == enrollmentId);
                if(enrollment == null) return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Enrollment not found.",
                    ErrorCode = 404
                };

                enrollment.IsDeleted = true;
                enrollment.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Enrollment deleted successfully.",
                    Data = $"Enrollment with Id:{enrollmentId} was deleted."
                };
            }
            catch (Exception e)
            {

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server Error. Try again.",
                    ErrorCode = 500,
                    Errors = new List<string> { e.Message , e.InnerException?.Message ?? "" }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .Include(e => e.Payments)
                    .AsQueryable();

                var totalCount = await query.CountAsync();
                var enrollments = await query
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!enrollments.Any())
                    return new ApiResponse<PagedResult<EnrollmentDetailsResponse>>
                    {
                        Success = false,
                        Message = "No enrollments are found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                var response = enrollments.Select(e => MapToEnrollmentDetailsResponse(e)).ToList();


                return new ApiResponse<PagedResult<EnrollmentDetailsResponse>>
                {
                    Success = true,
                    Message = "Enrollments retrieved successfully.",
                    Data = new PagedResult<EnrollmentDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<EnrollmentDetailsResponse>>
                {
                    Success = false,
                    Message = "Error retrieving enrollments.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        private EnrollmentDetailsResponse MapToEnrollmentDetailsResponse(Enrollment enrollment)
        {
            var (paymentStatus, totalPaid) = PaymentCalculator.Calculate(enrollment);

            return new EnrollmentDetailsResponse
            {
                Id = enrollment.Id,
                Student = new StudentListItemResponse
                {
                    Id = enrollment.Student!.Id,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email,
                    PhoneNumber = enrollment.Student.PhoneNumber,
                    IsActive = enrollment.Student.IsActive,
                    CreatedAt = enrollment.Student.CreatedAt
                },
                Track = new TrackBasicResponse
                {
                    Id = enrollment.TrainingTrack!.Id,
                    Title = enrollment.TrainingTrack.Title,
                    Level = enrollment.TrainingTrack.Level,
                    Price = enrollment.TrainingTrack.Price
                },
                Status = enrollment.Status,
                EnrollmentDate = enrollment.EnrollmentDate,
                FinalGrade = enrollment.FinalGrade,
                TotalPaid = totalPaid,
                PaymentStatus = paymentStatus,
                Payments = enrollment.Payments?
                    .Select(p => new PaymentResponse
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        PaymentMethod = p.PaymentMethod,
                        PaymentDate = p.PaymentDate,
                        PaymentStatus = p.PaymentStatus,
                        ReferenceNumber = p.ReferenceNumber
                    })
                    .ToList() ?? new List<PaymentResponse>()
            };
        }
    }
}
