using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services
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
                if (request.StudentId < 1) errors.Add("Student ID must be positive number.");
                if (request.TrainingTrackId < 1) errors.Add("Track ID must be positive number.");

                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
                if (student == null)
                    errors.Add("Student not found.");

                var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.Id == request.TrainingTrackId 
                                                                                && t.Status == TrackStatus.Published
                                                                                || t.Status == TrackStatus.Closed);
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

            
                if (!request.AllowInactiveStudent && !student.IsDeleted)
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Student is inactive or deleted and cannot be enrolled. Set AllowInactiveStudent=true to override." }
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

        public async Task<ApiResponse<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status)
        {
            try
            {
                var errors = new List<string>();
                if (id < 1) errors.Add("Enrollment ID must be positive number.");
                if (!Enum.IsDefined(status)) errors.Add($"This Status {status.ToString()} is not Defined.");

                if (errors.Any())
                    return new ApiResponse<EnrollmentDetailsResponse>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

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
                var errors = new List<string>();
                if (studentId < 1) errors.Add("Track ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<EnrollmentSummaryResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

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
                    TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate,
                    PaymentStatus = CalculatePaymentStatus(e.Id, e.TrainingTrack?.Price ?? 0,out var _)
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

        public async Task<ApiResponse<PagedResult<TrackEnrollmentStudents>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (trackId < 1) errors.Add("Track ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if(errors.Any())
                    return new ApiResponse<PagedResult<TrackEnrollmentStudents>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };


                var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.Id == trackId);
                if (track == null)
                    return new ApiResponse<PagedResult<TrackEnrollmentStudents>>
                    {
                        Success = false,
                        Message = "Track not found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                var query = _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.TrainingTrackId == trackId && 
                               (e.Status == EnrollmentStatus.Active || 
                                e.Status == EnrollmentStatus.Completed)) // only active or completed counts as enrolled
                    .AsQueryable();

                var totalCount = await query.CountAsync();
                var students = await query
                    .OrderBy(s => s.Student.FName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new TrackEnrollmentStudents
                    {
                        Id = s.Id,
                        FullName = s.Student.FullName,
                        Email = s.Student.Email,
                        PhoneNumber = s.Student.PhoneNumber,
                        IsActive = s.Student.IsActive,
                        CreatedAt = s.Student.CreatedAt,
                        EnrollmentStatus = s.Status,
                        EnrollmentDate = s.EnrollmentDate,
                    })
                    .ToListAsync();

                if (!students.Any())
                    return new ApiResponse<PagedResult<TrackEnrollmentStudents>>
                    {
                        Success = false,
                        Message = $"No Students are enrolled in the track id:{trackId}.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                return new ApiResponse<PagedResult<TrackEnrollmentStudents>>
                {
                    Success = true,
                    Message = "Track students retrieved successfully.",
                    Data = new PagedResult<TrackEnrollmentStudents>
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
                return new ApiResponse<PagedResult<TrackEnrollmentStudents>>
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
            var paymentStatus = CalculatePaymentStatus(enrollment.Id, trackPrice ,out var totalPaid);

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

        private PaymentStatus CalculatePaymentStatus(int enrollmentId, decimal trackPrice, out decimal totalPaid)
        {
            totalPaid = _context.Payments
                .Where(p => p.EnrollmentId == enrollmentId
                            && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.PartiallyPaid))
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
