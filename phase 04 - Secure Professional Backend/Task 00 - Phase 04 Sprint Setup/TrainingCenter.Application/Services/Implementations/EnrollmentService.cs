using TrainingCenter.Application.DTOs.Enrollment.Requests;
using TrainingCenter.Application.DTOs.Enrollment.Responses;
using TrainingCenter.Application.DTOs.Payment.Responses;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Application.Helpers;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10, EnrollmentStatus? status = null, int? trackId = null, int? studentId = null, PaymentStatus? paymentStatus = null)
        {
            try
            {
                var query = await _unitOfWork.Enrollments.GetAllAsync(includes:"Payments,Student,TrainingTrack");
                    
                if (status.HasValue)
                    query = query.Where(e => e.Status == status.Value);
                if (trackId.HasValue)
                    query = query.Where(e => e.TrainingTrackId == trackId.Value);
                if (studentId.HasValue)
                    query = query.Where(e => e.StudentId == studentId.Value);

                var totalCount = query.Count();
                var enrollments = query
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!enrollments.Any())
                    return Result<PagedResult<EnrollmentDetailsResponse>>
                        .FailureResult("No enrollments are found.", "Enrollment.NotFound");
                    
                       

                var response = enrollments.Select(e => MapToEnrollmentDetailsResponse(e)).ToList();

                if (paymentStatus.HasValue)
                    response = response.Where(r => r.PaymentStatus == paymentStatus.Value).ToList();


                return Result<PagedResult<EnrollmentDetailsResponse>>
                    .SuccessResult(new PagedResult<EnrollmentDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }, "Enrollments retrieved successfully.");
                
            }
            catch (Exception ex)
            {
                return Result<PagedResult<EnrollmentDetailsResponse>>
                    .FailureResult("Error retrieving enrollments.", ex.Message);
               
            }
        }

        public async Task<Result<EnrollmentDetailsResponse>> GetEnrollmentByIdAsync(int id)
        {
            try
            {
                if (id < 1) return Result<EnrollmentDetailsResponse>
                        .FailureResult("Enrollment ID must be positive number.", "Enrollment.BadRequest");

                var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(e => e.Id == id, includes: "Payments,Student,TrainingTrack");

                if (enrollment == null)
                    return Result<EnrollmentDetailsResponse>
                        .FailureResult("Enrollment not found.", "Enrollment.NotFound");

                return Result<EnrollmentDetailsResponse>
                    .SuccessResult(MapToEnrollmentDetailsResponse(enrollment), "Enrollment retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<EnrollmentDetailsResponse>
                    .FailureResult("Error retrieving enrollments.", ex.Message);

            }
        }

        public async Task<Result<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {
            try
            {
                var errors = new List<string>();
                if (request.StudentId < 1) errors.Add("Student ID must be positive number.");
                if (request.TrainingTrackId < 1) errors.Add("Track ID must be positive number.");

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == request.StudentId);
                if (student == null)
                    errors.Add("Student not found.");

                var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == request.TrainingTrackId
                                                                                && (t.Status == TrackStatus.Published
                                                                                || t.Status == TrackStatus.Closed));
                if (track == null)
                    errors.Add("Track not found.");

                if (errors.Any())
                    return Result<EnrollmentDetailsResponse>.FailureResult("Validation errors.", errors);
                    

                if (track!.Status == TrackStatus.Closed)
                    return Result<EnrollmentDetailsResponse>
                        .FailureResult("Validation errors.", "This track is closed and cannot accept new enrollments.");

                if (!request.AllowInactiveStudent && (student!.IsDeleted || !student.IsActive))
                    return Result<EnrollmentDetailsResponse>
                        .FailureResult("Validation errors.", "Student is inactive or deleted and cannot be enrolled. Set AllowInactiveStudent=true to override.");


                if (await _unitOfWork.Enrollments.ExistsAsync(e => e.StudentId == request.StudentId && e.TrainingTrackId == request.TrainingTrackId && e.Status != EnrollmentStatus.Cancelled))
                    return Result<EnrollmentDetailsResponse>
                        .FailureResult("Validation errors.", "Student is already enrolled in this track.");

                var enrolledCount = (await _unitOfWork.Enrollments.GetAllAsync
                    (e => e.TrainingTrackId == request.TrainingTrackId
                             && e.Status == EnrollmentStatus.Completed
                             || e.Status == EnrollmentStatus.Active)).Count();
                
                if (enrolledCount >= track!.Capacity)
                    return Result<EnrollmentDetailsResponse>
                        .FailureResult("Validation errors.", "Track capacity is full.");

                var enrollment = new Enrollment
                {
                    StudentId = request.StudentId,
                    TrainingTrackId = request.TrainingTrackId,
                    Status = EnrollmentStatus.Draft,
                    EnrollmentDate = DateTime.UtcNow
                };

                await _unitOfWork.Enrollments.AddAsync(enrollment);
                await _unitOfWork.SaveAsync();

                enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(e => e.Id == enrollment.Id, includes: "Payments,Student,TrainingTrack");
                    
                return Result<EnrollmentDetailsResponse>
                    .SuccessResult(MapToEnrollmentDetailsResponse(enrollment), "Enrollment created successfully.");
            }
            catch (Exception ex)
            {
                return Result<EnrollmentDetailsResponse>.FailureResult("Error creating enrollment.", ex.Message);
            }
        }

        public async Task<Result<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var errors = new List<string>();
                if (id < 1) errors.Add("Enrollment ID must be positive number.");
                if (!Enum.IsDefined(status)) errors.Add($"This Status {status.ToString()} is not Defined.");

                if (errors.Any())
                    return Result<EnrollmentDetailsResponse>.FailureResult("Validation errors.", errors);


                var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(e => e.Id == id, includes: "Payments,Student,TrainingTrack");

                if (enrollment == null)
                    return Result<EnrollmentDetailsResponse>.FailureResult("Enrollment not found.", "Enrollment.NotFound");

                // Validate status transitions
                if (!IsValidStatusTransition(enrollment.Status, status))
                    return Result<EnrollmentDetailsResponse>
                        .FailureResult("Invalid status transition.", $"Cannot transition from {enrollment.Status} to {status}.");

                enrollment.Status = status;
                if (status == EnrollmentStatus.Cancelled) RefundCanceledEnrollment(enrollment);
                
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Result<EnrollmentDetailsResponse>.SuccessResult(MapToEnrollmentDetailsResponse(enrollment), "Enrollment status updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<EnrollmentDetailsResponse>.FailureResult("Error updating enrollment status.", ex.Message);
            }
        }

        public async Task<Result<PagedResult<EnrollmentSummaryResponse>>> GetStudentEnrollmentsAsync(int studentId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (studentId < 1) errors.Add("Track ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<EnrollmentSummaryResponse>>.FailureResult("Validation errors.", errors);

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(e => e.Id == studentId);
                if (student == null)
                    return Result<PagedResult<EnrollmentSummaryResponse>>.FailureResult("Student not found.", "Student.NotFound");

                var query = await _unitOfWork.Enrollments.GetAllAsync(e => e.StudentId == studentId, includes: "Payments,TrainingTrack");


                var totalCount = query.Count();
                var enrollments =  query
                                .OrderByDescending(e => e.EnrollmentDate)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

                if (!enrollments.Any())
                    return Result<PagedResult<EnrollmentSummaryResponse>>
                        .FailureResult($"Student id:{studentId} has no enrollments.", "Enrollment.NotFound");

                var response = enrollments.Select(e => new EnrollmentSummaryResponse
                {
                    Id = e.Id,
                    TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate,
                    PaymentStatus = PaymentCalculator.Calculate(e).Status
                }).ToList();

                return Result<PagedResult<EnrollmentSummaryResponse>>.SuccessResult(new PagedResult<EnrollmentSummaryResponse>
                {
                    Items = response,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Student enrollments retrieved successfully.");
                
            }
            catch (Exception ex)
            {
                return Result<PagedResult<EnrollmentSummaryResponse>>
                    .FailureResult("Error retrieving student enrollments.", ex.Message);
            }
        }

        public async Task<Result<PagedResult<TrackEnrollmentStudents>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var errors = new List<string>();
                if (trackId < 1) errors.Add("Track ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<TrackEnrollmentStudents>>.FailureResult("Validation errors.", errors);


                var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == trackId);
                if (track == null)
                    return Result<PagedResult<TrackEnrollmentStudents>>.FailureResult("Track not found.", "Track.NotFound");

                var query = await _unitOfWork.Enrollments.GetAllAsync(e => e.TrainingTrackId == trackId &&
                               (e.Status == EnrollmentStatus.Active ||
                                e.Status == EnrollmentStatus.Completed), includes: "Student");

                var totalCount =  query.Count();
                var students = query
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
                    .ToList();

                if (!students.Any())
                    return Result<PagedResult<TrackEnrollmentStudents>>
                        .FailureResult($"No Students are enrolled in the track id:{trackId}.", "Track.NotFound");

                return Result<PagedResult<TrackEnrollmentStudents>>.SuccessResult(new PagedResult<TrackEnrollmentStudents>
                {
                    Items = students,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Track students retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<PagedResult<TrackEnrollmentStudents>>.FailureResult("Error retrieving track students.", ex.Message);
            }
        }
        private async void RefundCanceledEnrollment(Enrollment enrollment)
        {
            try
            {
                // Sum collected payments
                var totalCollected = enrollment.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Paid
                             || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                    .Sum(p => p.Amount);

                // Create refund row if any money was collected
                if (totalCollected > 0)
                {
                    await _unitOfWork.Payments.AddAsync(new Payment
                    {
                        EnrollmentId = enrollment.Id,
                        Amount = totalCollected,
                        PaymentMethod = PaymentMethod.BankTransfer,
                        PaymentDate = DateTime.UtcNow,
                        PaymentStatus = PaymentStatus.Refunded,
                        ReferenceNumber = $"REF-{enrollment.Id}-{Guid.NewGuid():N}"[..20],
                        Notes = $"Full refund on cancellation of enrollment #{enrollment.Id}"
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        private EnrollmentDetailsResponse MapToEnrollmentDetailsResponse(Enrollment enrollment)
        {
            (PaymentStatus paymentStatus, decimal totalPaid) = PaymentCalculator.Calculate(enrollment);

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
