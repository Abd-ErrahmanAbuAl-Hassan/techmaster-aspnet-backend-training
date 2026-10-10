using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingCenter.Application.DTOs.Payment.Requests;
using TrainingCenter.Application.DTOs.Payment.Responses;
using TrainingCenter.Application.Helpers;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    /// <summary>
    /// Service for managing payment operations including creation, retrieval, and status updates.
    /// Implements transaction management for payment processing.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, IEnrollmentService enrollmentService, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        public async Task<Result<PagedResult<PaymentResponse>>> GetPaymentsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            DateTime? startDate = null,
            DateTime? endDate = null,
            PaymentStatus? status = null)
        {
            try
            {
                _logger.LogInformation("Fetching payments. Page: {PageNumber}, Size: {PageSize}, StartDate: {StartDate}, EndDate: {EndDate}, Status: {Status}",
                    pageNumber, pageSize, startDate, endDate, status);

                var errors = new List<string>();
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");
                if (startDate.HasValue != endDate.HasValue)
                    errors.Add("Both start and end dates must be provided together or both omitted.");
                if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                    errors.Add(ResultMessages.Validation.DateRangeInvalid);
                if (status.HasValue && !Enum.IsDefined(typeof(PaymentStatus), status.Value))
                    errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Payment status"));

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetPaymentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<PaymentResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var query = await _unitOfWork.Payments.GetAllAsync();

                if (startDate.HasValue)
                    query = query.Where(p => p.PaymentDate >= startDate.Value);
                if (endDate.HasValue)
                    query = query.Where(p => p.PaymentDate <= endDate.Value);
                if (status.HasValue)
                    query = query.Where(p => p.PaymentStatus == status.Value);

                var totalCount = query.Count();
                var payments = query
                    .OrderByDescending(p => p.PaymentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PaymentResponse
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        PaymentMethod = p.PaymentMethod,
                        PaymentDate = p.PaymentDate,
                        PaymentStatus = p.PaymentStatus,
                        ReferenceNumber = p.ReferenceNumber
                    })
                    .ToList();

                if (!payments.Any())
                {
                    _logger.LogInformation("No payments found with the specified criteria.");
                    return Result<PagedResult<PaymentResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} payments.", payments.Count);

                return Result<PagedResult<PaymentResponse>>.SuccessResult(
                    new PagedResult<PaymentResponse>
                    {
                        Items = payments,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    string.Format(ResultMessages.Success.ResourceRetrieved, "Payments"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payments. Exception: {@Exception}", ex);
                return Result<PagedResult<PaymentResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation("Creating payment. EnrollmentId: {EnrollmentId}, Amount: {Amount}, UserId: {UserId}",
                        request.EnrollmentId, request.Amount, request.UserId);

                    var errors = new List<string>();
                    if (request.EnrollmentId < 1)
                        errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Enrollment ID"));
                    if (request.Amount <= 0)
                        errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Amount"));
                    if (!Enum.IsDefined(request.PaymentMethod))
                        errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Payment method"));

                    if (errors.Any())
                    {
                        _logger.LogWarning("Validation failed for CreatePaymentAsync. Errors: {@Errors}", errors);
                        return Result<PaymentResponse>.ValidationErrorResult(
                            "Validation errors occurred. Please review the details below.",
                            errors);
                    }

                    var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(
                        e => e.Id == request.EnrollmentId,
                        "TrainingTrack");

                    if (enrollment == null)
                    {
                        _logger.LogInformation("Enrollment not found. ID: {EnrollmentId}", request.EnrollmentId);
                        return Result<PaymentResponse>.NotFoundResult(
                            string.Format(ResultMessages.NotFound.ResourceNotFound, "Enrollment"));
                    }

                    if (enrollment.StudentId != request.UserId)
                    {
                        _logger.LogWarning("User trying to pay for enrollment they don't own. UserId: {UserId}, EnrollmentId: {EnrollmentId}", request.UserId, request.EnrollmentId);
                        return Result<PaymentResponse>.ForbiddenResult(
                            ResultMessages.Unauthorized.AccessDenied);
                    }

                    if (enrollment.Status == EnrollmentStatus.Cancelled)
                    {
                        _logger.LogWarning("Cannot create payment for cancelled enrollment. EnrollmentId: {EnrollmentId}", enrollment.Id);
                        return Result<PaymentResponse>.ConflictResult(
                            "Cannot create a payment for a cancelled enrollment.");
                    }

                    var totalPaid = (await _unitOfWork.Payments.GetAllAsync(
                        p => p.EnrollmentId == request.EnrollmentId &&
                             (p.PaymentStatus == PaymentStatus.PartiallyPaid ||
                              p.PaymentStatus == PaymentStatus.Paid)))
                        .Sum(p => p.Amount);

                    decimal totalAmount = totalPaid + request.Amount;

                    if (totalPaid >= enrollment.TrainingTrack!.Price)
                    {
                        _logger.LogWarning("Enrollment is already fully paid. EnrollmentId: {EnrollmentId}", enrollment.Id);
                        return Result<PaymentResponse>.ConflictResult(
                            "The enrollment has already been fully paid.");
                    }

                    PaymentStatus status = PaymentStatus.Pending;

                    if (totalAmount > enrollment!.TrainingTrack!.Price)
                    {
                        _logger.LogWarning("Payment amount exceeds track price. EnrollmentId: {EnrollmentId}, PaymentAmount: {PaymentAmount}, TrackPrice: {TrackPrice}",
                            enrollment.Id, request.Amount, enrollment.TrainingTrack.Price);
                        return Result<PaymentResponse>.ConflictResult(
                            ResultMessages.Conflict.InsufficientFunds);
                    }
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

                    await _unitOfWork.Payments.AddAsync(payment);

                    if (totalAmount >= enrollment.TrainingTrack!.Price)
                    {
                        enrollment.Status = EnrollmentStatus.Active;
                        _logger.LogInformation("Enrollment activated due to full payment. EnrollmentId: {EnrollmentId}", enrollment.Id);
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Successfully created payment. ID: {PaymentId}, EnrollmentId: {EnrollmentId}, Amount: {Amount}",
                        payment.Id, request.EnrollmentId, request.Amount);

                    return Result<PaymentResponse>.SuccessResult(
                        new PaymentResponse
                        {
                            Id = payment.Id,
                            Amount = payment.Amount,
                            PaymentMethod = payment.PaymentMethod,
                            PaymentDate = payment.PaymentDate,
                            PaymentStatus = payment.PaymentStatus,
                            ReferenceNumber = payment.ReferenceNumber
                        },
                        string.Format(ResultMessages.Success.ResourceCreated, "Payment"),
                        201);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "An error occurred while creating payment. Exception: {@Exception}", ex);
                    return Result<PaymentResponse>.FailureResult(
                        ResultMessages.ServerError.PaymentProcessingError,
                        ex.Message);
                }
            });
        }

        public async Task<Result<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching enrollment payments. EnrollmentId: {EnrollmentId}, Page: {PageNumber}, Size: {PageSize}",
                    enrollmentId, pageNumber, pageSize);

                var errors = new List<string>();
                if (enrollmentId < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Enrollment ID"));
                if (pageNumber < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page number"));
                if (pageSize < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Page size"));
                if (pageSize > 50)
                    errors.Add("Page size must be at most 50.");

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for GetEnrollmentPaymentsAsync. Errors: {@Errors}", errors);
                    return Result<PagedResult<PaymentResponse>>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(e => e.Id == enrollmentId);
                if (enrollment == null)
                {
                    _logger.LogInformation("Enrollment not found. ID: {EnrollmentId}", enrollmentId);
                    return Result<PagedResult<PaymentResponse>>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Enrollment"));
                }

                var query = await _unitOfWork.Payments.GetAllAsync(p => p.EnrollmentId == enrollmentId);
                var totalCount = query.Count();
                var payments = query
                    .OrderByDescending(p => p.PaymentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PaymentResponse
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        PaymentMethod = p.PaymentMethod,
                        PaymentDate = p.PaymentDate,
                        PaymentStatus = p.PaymentStatus,
                        ReferenceNumber = p.ReferenceNumber,
                        Notes = p.Notes,
                    })
                    .ToList();

                if (!payments.Any())
                {
                    _logger.LogInformation("No payments found for enrollment. EnrollmentId: {EnrollmentId}", enrollmentId);
                    return Result<PagedResult<PaymentResponse>>.NotFoundResult(
                        ResultMessages.NotFound.RecordNotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} payments for enrollment. EnrollmentId: {EnrollmentId}", payments.Count, enrollmentId);

                return Result<PagedResult<PaymentResponse>>.SuccessResult(
                    new PagedResult<PaymentResponse>
                    {
                        Items = payments,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    },
                    "Enrollment payments retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving enrollment payments. EnrollmentId: {EnrollmentId}, Exception: {@Exception}", enrollmentId, ex);
                return Result<PagedResult<PaymentResponse>>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        public async Task<Result<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status)
        {
            try
            {
                _logger.LogInformation("Updating payment status. PaymentId: {PaymentId}, NewStatus: {Status}", id, status);

                var errors = new List<string>();
                if (id < 1)
                    errors.Add(string.Format(ResultMessages.Validation.PositiveNumber, "Payment ID"));
                if (!Enum.IsDefined(status))
                    errors.Add(string.Format(ResultMessages.Validation.EnumInvalid, "Payment status"));

                if (errors.Any())
                {
                    _logger.LogWarning("Validation failed for UpdatePaymentStatusAsync. Errors: {@Errors}", errors);
                    return Result<PaymentResponse>.ValidationErrorResult(
                        "Validation errors occurred. Please review the details below.",
                        errors);
                }

                var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(p => p.Id == id);
                if (payment == null)
                {
                    _logger.LogInformation("Payment not found. ID: {PaymentId}", id);
                    return Result<PaymentResponse>.NotFoundResult(
                        string.Format(ResultMessages.NotFound.ResourceNotFound, "Payment"));
                }

                if (!IsValidStatusTransition(payment.PaymentStatus, status))
                {
                    _logger.LogWarning("Invalid status transition. Current: {CurrentStatus}, Requested: {NewStatus}", payment.PaymentStatus, status);
                    return Result<PaymentResponse>.ConflictResult(
                        $"Cannot transition from {payment.PaymentStatus} to {status}.");
                }

                payment.PaymentStatus = status;
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully updated payment status. PaymentId: {PaymentId}, NewStatus: {Status}", id, status);

                return Result<PaymentResponse>.SuccessResult(
                    new PaymentResponse
                    {
                        Id = payment.Id,
                        Amount = payment.Amount,
                        PaymentMethod = payment.PaymentMethod,
                        PaymentDate = payment.PaymentDate,
                        PaymentStatus = payment.PaymentStatus,
                        ReferenceNumber = payment.ReferenceNumber,
                        Notes = payment.Notes,
                    },
                    "Payment status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating payment status. PaymentId: {PaymentId}, Exception: {@Exception}", id, ex);
                return Result<PaymentResponse>.FailureResult(
                    ResultMessages.ServerError.ProcessingError,
                    ex.Message);
            }
        }

        private bool IsValidStatusTransition(PaymentStatus currentStatus, PaymentStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (PaymentStatus.Pending, PaymentStatus.Paid) => true,
                (PaymentStatus.Pending, PaymentStatus.PartiallyPaid) => true,
                (PaymentStatus.Pending, PaymentStatus.Failed) => true,
                (PaymentStatus.PartiallyPaid, PaymentStatus.Paid) => true,
                (PaymentStatus.PartiallyPaid, PaymentStatus.Failed) => true,
                (PaymentStatus.Paid, PaymentStatus.Refunded) => true,
                _ => false
            };
        }
    }
}
