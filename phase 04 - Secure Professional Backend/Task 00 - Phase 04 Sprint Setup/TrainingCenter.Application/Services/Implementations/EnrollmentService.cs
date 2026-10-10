using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(IUnitOfWork unitOfWork, ILogger<EnrollmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            EnrollmentStatus? status = null,
            int? trackId = null,
            int? studentId = null,
            PaymentStatus? paymentStatus = null)
        {
            try
            {
                _logger.LogInformation("Fetching enrollments. Page: {PageNumber}, Size: {PageSize}, Status: {Status}, TrackId: {TrackId}, StudentId: {StudentId}, PaymentStatus: {PaymentStatus}",
                    pageNumber, pageSize, status, trackId, studentId, paymentStatus);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetEnrollmentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<EnrollmentDetailsResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var query = await _unitOfWork.Enrollments.GetAllAsync(includes: "Payments,Student,TrainingTrack");

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
                {
                    _logger.LogInformation("No enrollments found with the specified criteria.");
                    return Result<PagedResult<EnrollmentDetailsResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                var response = enrollments.Select(e => MapToEnrollmentDetailsResponse(e)).ToList();

                if (paymentStatus.HasValue)
                    response = response.Where(r => r.PaymentStatus == paymentStatus.Value).ToList();

                _logger.LogInformation("Successfully retrieved {Count} enrollments.", response.Count);

                return Result<PagedResult<EnrollmentDetailsResponse>>.SuccessResult(
                    new PagedResult<EnrollmentDetailsResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Enrollments"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving enrollments. Exception: {@Exception}", ex);
                return Result<PagedResult<EnrollmentDetailsResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<EnrollmentDetailsResponse>> GetEnrollmentByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching enrollment with ID: {EnrollmentId}", id);

                if (id < 1)
                {
                    _logger.LogWarning("Invalid enrollment ID: {EnrollmentId}", id);
                    return Result<EnrollmentDetailsResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        new List<string> { string.Format(ResultMessages.Validation.PositiveNumber, "Enrollment ID") });
                }

                var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(
                    e => e.Id == id,
                    includes: "Payments,Student,TrainingTrack");

                if (enrollment == null)
                {
                    _logger.LogInformation("Enrollment not found. ID: {EnrollmentId}", id);
                    return Result<EnrollmentDetailsResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Enrollment"));
                }

                _logger.LogInformation("Successfully retrieved enrollment. ID: {EnrollmentId}", id);

                return Result<EnrollmentDetailsResponse>.SuccessResult(
                    MapToEnrollmentDetailsResponse(enrollment),
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Enrollment"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving enrollment by ID: {EnrollmentId}. Exception: {@Exception}", id, ex);
                return Result<EnrollmentDetailsResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new enrollment. StudentId: {StudentId}, TrackId: {TrackId}",
                    request.StudentId, request.TrainingTrackId);

                var errors = new List<string>();
                if (request.StudentId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Student ID"));
                if (request.TrainingTrackId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(s => s.Id == request.StudentId);
                if (student == null)
                    errors.Add(string.Format(ResultMessages.NotFound.ResourceNotFound, "Student"));

                var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == request.TrainingTrackId
                                                                                && (t.Status == TrackStatus.Published
                                                                                || t.Status == TrackStatus.Closed));
                if (track == null)
                    errors.Add(string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track"));

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for CreateEnrollmentAsync. Errors: {@Errors}", errors);
                    return Result<EnrollmentDetailsResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                if (track!.Status == TrackStatus.Closed)
                {
                    _logger.LogWarning("Track is closed and cannot accept new enrollments. TrackId: {TrackId}", track.Id);
                    return Result<EnrollmentDetailsResponse>.ConflictResult(
                        ResultMessages.Conflict.InvalidTrackStatus);
                }

                if (!request.AllowInactiveStudent && (student!.IsDeleted || !student.IsActive))
                {
                    _logger.LogWarning("Student is inactive or deleted. StudentId: {StudentId}", student.Id);
                    return Result<EnrollmentDetailsResponse>.ConflictResult(
                        "Student is inactive or deleted. Please activate the student account before enrollment.");
                }

                if (await _unitOfWork.Enrollments.ExistsAsync(e => e.StudentId == request.StudentId
                    && e.TrainingTrackId == request.TrainingTrackId
                    && e.Status != EnrollmentStatus.Cancelled))
                {
                    _logger.LogWarning("Student is already enrolled in this track. StudentId: {StudentId}, TrackId: {TrackId}",
                        request.StudentId, request.TrainingTrackId);
                    return Result<EnrollmentDetailsResponse>.ConflictResult(
                        ResultMessages.Conflict.AlreadyEnrolled);
                }

                var enrolledCount = (await _unitOfWork.Enrollments.GetAllAsync(
                    e => e.TrainingTrackId == request.TrainingTrackId
                         && (e.Status == EnrollmentStatus.Completed || e.Status == EnrollmentStatus.Active))).Count();

                if (enrolledCount >= track!.Capacity)
                {
                    _logger.LogWarning("Track capacity is full. TrackId: {TrackId}, Capacity: {Capacity}",
                        track.Id, track.Capacity);
                    return Result<EnrollmentDetailsResponse>.ConflictResult(
                        ResultMessages.Conflict.InsufficientCapacity);
                }

                var enrollment = new Enrollment
                {
                    StudentId = request.StudentId,
                    TrainingTrackId = request.TrainingTrackId,
                    Status = EnrollmentStatus.Draft,
                    EnrollmentDate = DateTime.UtcNow
                };

                await _unitOfWork.Enrollments.AddAsync(enrollment);
                await _unitOfWork.SaveAsync();

                enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(
                    e => e.Id == enrollment.Id,
                    includes: "Payments,Student,TrainingTrack");

                _logger.LogInformation("Successfully created enrollment. ID: {EnrollmentId}, StudentId: {StudentId}, TrackId: {TrackId}",
                    enrollment.Id, request.StudentId, request.TrainingTrackId);

                return Result<EnrollmentDetailsResponse>.SuccessResult(
                    MapToEnrollmentDetailsResponse(enrollment),
                    string.Format(ResultMessages.Success.ResourceCreated, "Enrollment"),
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating enrollment. Exception: {@Exception}", ex);
                return Result<EnrollmentDetailsResponse>.FailureResult(
                    ResultMessages.ServerError.DatabaseError,
                    ex.Message);
            }
        }

        public async Task<Result<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Updating enrollment status. EnrollmentId: {EnrollmentId}, NewStatus: {Status}", id, status);

                var errors = new List<string>();
                if (id < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Enrollment ID"));
                if (!Enum.IsDefined(status))
                    errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Enrollment status"));

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for UpdateEnrollmentStatusAsync. Errors: {@Errors}", errors);
                    return Result<EnrollmentDetailsResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(
                    e => e.Id == id,
                    includes: "Payments,Student,TrainingTrack");

                if (enrollment == null)
                {
                    _logger.LogInformation("Enrollment not found for status update. ID: {EnrollmentId}", id);
                    return Result<EnrollmentDetailsResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Enrollment"));
                }

                if (!IsValidStatusTransition(enrollment.Status, status))
                {
                    _logger.LogWarning("Invalid status transition. Current: {CurrentStatus}, Requested: {NewStatus}", enrollment.Status, status);
                    return Result<EnrollmentDetailsResponse>.ConflictResult(
                        $"Cannot transition from {enrollment.Status} to {status}.");
                }

                enrollment.Status = status;
                if (status == EnrollmentStatus.Cancelled)
                    await RefundCanceledEnrollmentAsync(enrollment);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated enrollment status. EnrollmentId: {EnrollmentId}, NewStatus: {Status}", id, status);

                return Result<EnrollmentDetailsResponse>.SuccessResult(
                    MapToEnrollmentDetailsResponse(enrollment),
                    "Enrollment status updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while updating enrollment status. ID: {EnrollmentId}, Exception: {@Exception}", id, ex);
                return Result<EnrollmentDetailsResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<EnrollmentSummaryResponse>>> GetStudentEnrollmentsAsync(int studentId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching student enrollments. StudentId: {StudentId}, Page: {PageNumber}, Size: {PageSize}",
                    studentId, pageNumber, pageSize);

                var errors = new List<string>();
                if (studentId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Student ID"));
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetStudentEnrollmentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<EnrollmentSummaryResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var student = await _unitOfWork.Students.GetFirstOrDefaultAsync(e => e.Id == studentId);
                if (student == null)
                {
                    _logger.LogInformation("Student not found. ID: {StudentId}", studentId);
                    return Result<PagedResult<EnrollmentSummaryResponse>>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Student"));
                }

                var query = await _unitOfWork.Enrollments.GetAllAsync(
                    e => e.StudentId == studentId,
                    includes: "Payments,TrainingTrack");

                var totalCount = query.Count();
                var enrollments = query
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!enrollments.Any())
                {
                    _logger.LogInformation("No enrollments found for student. StudentId: {StudentId}", studentId);
                    return Result<PagedResult<EnrollmentSummaryResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                var response = enrollments.Select(e => new EnrollmentSummaryResponse
                {
                    Id = e.Id,
                    TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate,
                    PaymentStatus = PaymentCalculator.Calculate(e).Status
                }).ToList();

                _logger.LogInformation("Successfully retrieved {Count} enrollments for student. StudentId: {StudentId}", response.Count, studentId);

                return Result<PagedResult<EnrollmentSummaryResponse>>.SuccessResult(
                    new PagedResult<EnrollmentSummaryResponse>
                    {
                        Items = response,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Student enrollments retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving student enrollments. StudentId: {StudentId}, Exception: {@Exception}", studentId, ex);
                return Result<PagedResult<EnrollmentSummaryResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PagedResult<TrackEnrollmentStudents>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching track students. TrackId: {TrackId}, Page: {PageNumber}, Size: {PageSize}",
                    trackId, pageNumber, pageSize);

                var errors = new List<string>();
                if (trackId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Track ID"));
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetTrackStudentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<TrackEnrollmentStudents>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var track = await _unitOfWork.Tracks.GetFirstOrDefaultAsync(t => t.Id == trackId);
                if (track == null)
                {
                    _logger.LogInformation("Track not found. ID: {TrackId}", trackId);
                    return Result<PagedResult<TrackEnrollmentStudents>>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Training track"));
                }

                var query = await _unitOfWork.Enrollments.GetAllAsync(
                    e => e.TrainingTrackId == trackId &&
                         (e.Status == EnrollmentStatus.Active ||
                          e.Status == EnrollmentStatus.Completed),
                    includes: "Student");

                var totalCount = query.Count();
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
                {
                    _logger.LogInformation("No enrolled students found in track. TrackId: {TrackId}", trackId);
                    return Result<PagedResult<TrackEnrollmentStudents>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} students for track. TrackId: {TrackId}", students.Count, trackId);

                return Result<PagedResult<TrackEnrollmentStudents>>.SuccessResult(
                    new PagedResult<TrackEnrollmentStudents>
                    {
                        Items = students,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Track students retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving track students. TrackId: {TrackId}, Exception: {@Exception}", trackId, ex);
                return Result<PagedResult<TrackEnrollmentStudents>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        private async Task RefundCanceledEnrollmentAsync(Enrollment enrollment)
        {
            try
            {
                _logger.LogInformation("Processing refund for canceled enrollment. EnrollmentId: {EnrollmentId}", enrollment.Id);

                var totalCollected = enrollment.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Paid
                             || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                    .Sum(p => p.Amount);

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

                    _logger.LogInformation("Refund processed. EnrollmentId: {EnrollmentId}, Amount: {Amount}", enrollment.Id, totalCollected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing refund. EnrollmentId: {EnrollmentId}, Exception: {@Exception}", enrollment.Id, ex);
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
                (EnrollmentStatus.Draft, EnrollmentStatus.Active) => true,
                (EnrollmentStatus.Draft, EnrollmentStatus.Cancelled) => true,
                (EnrollmentStatus.Active, EnrollmentStatus.Completed) => true,
                (EnrollmentStatus.Active, EnrollmentStatus.Cancelled) => true,
                _ => false
            };
        }
    }
}
