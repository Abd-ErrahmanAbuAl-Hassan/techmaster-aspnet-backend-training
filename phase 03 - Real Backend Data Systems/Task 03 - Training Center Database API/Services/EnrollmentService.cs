using Microsoft.EntityFrameworkCore;
using Task_03___Training_Center_Database_API.Data;
using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Entities;
using Task_03___Training_Center_Database_API.Services.Interfaces;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10, EnrollmentStatus? status = null, int? trackId = null, int? studentId = null, PaymentStatus? paymentStatus = null)
        {
            try
            {
                var query = _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .Include(e => e.Payments)
                    .AsQueryable();

                if (status.HasValue)
                    query = query.Where(e => e.Status == status.Value);
                if (trackId.HasValue)
                    query = query.Where(e => e.TrainingTrackId == trackId.Value);
                if (studentId.HasValue)
                    query = query.Where(e => e.StudentId == studentId.Value);

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

                if (paymentStatus.HasValue)
                    response = response.Where(r => r.PaymentStatus == paymentStatus.Value).ToList();

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

        public async Task<ApiResponse<EnrollmentDetailsResponse>> GetEnrollmentByIdAsync(int id)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .Include(e => e.Payments)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (enrollment == null)
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Enrollment not found.",
                        ErrorCode = 404
                    };

                return new ApiResponse<EnrollmentDetailsResponse>
                {
                    Success = true,
                    Message = "Enrollment retrieved successfully.",
                    Data = MapToEnrollmentDetailsResponse(enrollment)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EnrollmentDetailsResponse>
                {
                    Success = false,
                    Message = "Error retrieving enrollment.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {
            try
            {
                var errors = new List<string>();

                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
                if (student == null)
                    errors.Add("Student not found.");

                var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.Id == request.TrainingTrackId);
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

                // Check for duplicate enrollment
                if (await _context.Enrollments.AnyAsync(e => e.StudentId == request.StudentId && e.TrainingTrackId == request.TrainingTrackId && e.Status != EnrollmentStatus.Cancelled))
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Student is already enrolled in this track." }
                    };

                // Check capacity
                var enrolledCount = await _context.Enrollments
                    .Where(e => e.TrainingTrackId == request.TrainingTrackId && e.Status != EnrollmentStatus.Cancelled)
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

        public async Task<ApiResponse<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.TrainingTrack)
                    .Include(e => e.Payments)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (enrollment == null)
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Enrollment not found.",
                        ErrorCode = 404
                    };

                // Validate status transitions
                if (!IsValidStatusTransition(enrollment.Status, status))
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Invalid status transition.",
                        ErrorCode = 400,
                        Errors = new List<string> { $"Cannot transition from {enrollment.Status} to {status}." }
                    };

                enrollment.Status = status;
                await _context.SaveChangesAsync();

                return new ApiResponse<EnrollmentDetailsResponse>
                {
                    Success = true,
                    Message = "Enrollment status updated successfully.",
                    Data = MapToEnrollmentDetailsResponse(enrollment)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EnrollmentDetailsResponse>
                {
                    Success = false,
                    Message = "Error updating enrollment status.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<EnrollmentSummaryResponse>>> GetStudentEnrollmentsAsync(int studentId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
                if (student == null)
                    return new ApiResponse<PagedResult<EnrollmentSummaryResponse>>
                    {
                        Success = false,
                        Message = "Student not found.",
                        ErrorCode = 404
                    };

                var query = _context.Enrollments
                    .Include(e => e.TrainingTrack)
                    .Where(e => e.StudentId == studentId);

                var totalCount = await query.CountAsync();
                var enrollments = await query
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!enrollments.Any())
                    return new ApiResponse<PagedResult<EnrollmentSummaryResponse>>
                    {
                        Success = false,
                        Message = $"Student id:{studentId} has no enrollments.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                var response = enrollments.Select(e => new EnrollmentSummaryResponse
                {
                    Id = e.Id,
                    TrackName = e.TrainingTrack?.Title ?? string.Empty,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate,
                    PaymentStatus = CalculatePaymentStatus(e.Id, e.TrainingTrack?.Price ?? 0)
                }).ToList();

                return new ApiResponse<PagedResult<EnrollmentSummaryResponse>>
                {
                    Success = true,
                    Message = "Student enrollments retrieved successfully.",
                    Data = new PagedResult<EnrollmentSummaryResponse>
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
                return new ApiResponse<PagedResult<EnrollmentSummaryResponse>>
                {
                    Success = false,
                    Message = "Error retrieving student enrollments.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.Id == trackId);
                if (track == null)
                    return new ApiResponse<PagedResult<StudentListItemResponse>>
                    {
                        Success = false,
                        Message = "Track not found.",
                        ErrorCode = 404
                    };

                var query = _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.TrainingTrackId == trackId)
                    .Select(e => e.Student);

                var totalCount = await query.CountAsync();
                var students = await query
                    .OrderBy(s => s.FName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new StudentListItemResponse
                    {
                        Id = s.Id,
                        FullName = s.FullName,
                        Email = s.Email,
                        PhoneNumber = s.PhoneNumber,
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                if (!students.Any())
                    return new ApiResponse<PagedResult<StudentListItemResponse>>
                    {
                        Success = false,
                        Message = $"No Students are enrolled in the track id:{trackId}.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };
                return new ApiResponse<PagedResult<StudentListItemResponse>>
                {
                    Success = true,
                    Message = "Track students retrieved successfully.",
                    Data = new PagedResult<StudentListItemResponse>
                    {
                        Items = students,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<StudentListItemResponse>>
                {
                    Success = false,
                    Message = "Error retrieving track students.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private EnrollmentDetailsResponse MapToEnrollmentDetailsResponse(Enrollment enrollment)
        {
            var trackPrice = enrollment.TrainingTrack?.Price ?? 0;
            var paymentStatus = CalculatePaymentStatus(enrollment.Id, trackPrice);
            var totalPaid = enrollment.Payments?
                .Where(p => p.PaymentStatus != PaymentStatus.Pending)
                .Sum(p => p.Amount) ?? 0;

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
                    Name = enrollment.TrainingTrack.Title,
                    Level = enrollment.TrainingTrack.Level,
                    Price = enrollment.TrainingTrack.Price
                },
                Status = enrollment.Status,
                EnrollmentDate = enrollment.EnrollmentDate,
                FinalGrade = enrollment.FinalResult,
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

        private PaymentStatus CalculatePaymentStatus(int enrollmentId, decimal trackPrice)
        {
            var totalPaid = _context.Payments
                .Where(p => p.EnrollmentId == enrollmentId && p.PaymentStatus != PaymentStatus.Pending)
                .Sum(p => p.Amount);

            if (totalPaid >= trackPrice)
                return PaymentStatus.Paid;
            else if (totalPaid > 0)
                return PaymentStatus.PartiallyPaid;
            else
                return PaymentStatus.Pending;
        }

        private bool IsValidStatusTransition(EnrollmentStatus currentStatus, EnrollmentStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (EnrollmentStatus.Active, EnrollmentStatus.Completed) => true,
                (EnrollmentStatus.Draft, EnrollmentStatus.Active) => true,
                (EnrollmentStatus.Draft, EnrollmentStatus.Cancelled) => true,
                _ => false
            };
        }
    }
}
