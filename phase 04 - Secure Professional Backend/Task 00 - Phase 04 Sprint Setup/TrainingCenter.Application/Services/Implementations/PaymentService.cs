using Microsoft.EntityFrameworkCore;
using TrainingCenter.Application.DTOs.Payment.Requests;
using TrainingCenter.Application.DTOs.Payment.Responses;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Application.Services.Interfaces;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnrollmentService _enrollmentService;
        public PaymentService(IUnitOfWork unitOfWork, IEnrollmentService enrollmentService)
        {
            _unitOfWork = unitOfWork;
            _enrollmentService = enrollmentService;
        }

        public async Task<Result<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, 
                                                                                      DateTime? startDate = null, DateTime? endDate = null,
                                                                                      PaymentStatus? status = null)
        {
            try
            {

                var errors = new List<string>();
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");
                if(startDate.HasValue != endDate.HasValue) errors.Add("Both start and end dates must be provided together.");
                if(startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value) errors.Add("Start date must be less than or equal End date.");
                if (status.HasValue && !Enum.IsDefined(typeof(PaymentStatus), status.Value)) errors.Add("Payment status is invalid.");

                if (errors.Any())
                    return Result<PagedResult<PaymentResponse>>.FailureResult("Validation errors.", errors);

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
                    return Result<PagedResult<PaymentResponse>>.FailureResult("No payments are found.", ".NotFound");

                return Result<PagedResult<PaymentResponse>>.SuccessResult(new PagedResult<PaymentResponse>
                {
                    Items = payments,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Payments retrieved successfully.");

            }
            catch (Exception ex)
            {
                return Result<PagedResult<PaymentResponse>>.FailureResult("Error retrieving payments.", ex.Message);
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
                    var errors = new List<string>();
                    if (request.EnrollmentId < 1) errors.Add("Enrollment ID must be positive number.");
                    if (request.Amount <= 0) errors.Add("Amount must be greater than zero.");
                    if (!Enum.IsDefined(request.PaymentMethod)) errors.Add("Payment method not valid.");

                    if (errors.Any())
                        return Result<PaymentResponse>.FailureResult("Validation errors.", errors);

                    var enrollment = await _unitOfWork.Enrollments
                                    .GetFirstOrDefaultAsync(e => e.Id == request.EnrollmentId, "TrainingTrack");

                    if (enrollment == null) return Result<PaymentResponse>.FailureResult("Enrollment not found.", statusCode: 404);
                    errors.Add("Enrollment not found.");

                    if(enrollment.StudentId != request.UserId) return Result<PaymentResponse>.FailureResult("You can only pay your enrollments.", statusCode: 403);
                    if (enrollment.Status == EnrollmentStatus.Cancelled)
                        return Result<PaymentResponse>.FailureResult("Validation errors.", "Cannot create a payment for a cancelled enrollment.");

                    var totalPaid = (await _unitOfWork.Payments
                            .GetAllAsync(p => p.EnrollmentId == request.EnrollmentId &&
                                  (p.PaymentStatus == PaymentStatus.PartiallyPaid ||
                                   p.PaymentStatus == PaymentStatus.Paid)))
                            .Sum(p => p.Amount);

                    decimal totalAmount = totalPaid + request.Amount;

                    if (totalPaid >= enrollment.TrainingTrack!.Price)
                        return Result<PaymentResponse>.FailureResult("The enrollment is already paid.", "Conflict error");

                    PaymentStatus status = PaymentStatus.Pending;

                    if (totalAmount > enrollment!.TrainingTrack!.Price)
                        return Result<PaymentResponse>.FailureResult("Validation errors.", "Payment amount exceeds track price.");
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
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return Result<PaymentResponse>.SuccessResult(new PaymentResponse
                    {
                        Id = payment.Id,
                        Amount = payment.Amount,
                        PaymentMethod = payment.PaymentMethod,
                        PaymentDate = payment.PaymentDate,
                        PaymentStatus = payment.PaymentStatus,
                        ReferenceNumber = payment.ReferenceNumber
                    }, "Payment created successfully.");

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<PaymentResponse>.FailureResult("Error creating payment.", ex.Message);
                }
            });
        }
        public async Task<Result<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                var errors = new List<string>();
                if (enrollmentId < 1) errors.Add("Enrollment ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return Result<PagedResult<PaymentResponse>>.FailureResult("Validation errors.", errors);

                var enrollment = await _unitOfWork.Enrollments.GetFirstOrDefaultAsync(e => e.Id == enrollmentId);
                if (enrollment == null)
                    return Result<PagedResult<PaymentResponse>>.FailureResult("Enrollment not found.", ".NotFound");

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
                    return Result<PagedResult<PaymentResponse>>.FailureResult("No payments are found.", ".NotFound");

                return Result<PagedResult<PaymentResponse>>.SuccessResult(new PagedResult<PaymentResponse>
                {
                    Items = payments,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, "Enrollment payments retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<PagedResult<PaymentResponse>>.FailureResult( "Error retrieving enrollment payments.", ex.Message);
            }
        }
        public async Task<Result<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status)
        {
            try
            {

                var errors = new List<string>();
                if (id < 1) errors.Add("Payment ID must be positive number.");
                if(!Enum.IsDefined(status)) errors.Add("Payment status is invalid.");

                if (errors.Any())
                    return Result<PaymentResponse>.FailureResult("Validation errors.", errors);

                var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(p => p.Id == id);
                if (payment == null)
                    return Result<PaymentResponse>.FailureResult("Payment not found.", ".NotFound");

                // Validate status transitions
                if (!IsValidStatusTransition(payment.PaymentStatus, status))
                    return Result<PaymentResponse>.FailureResult("Invalid status transition.", $"Cannot transition from {payment.PaymentStatus} to {status}.");

                payment.PaymentStatus = status;
                await _unitOfWork.SaveAsync();

                return Result<PaymentResponse>.SuccessResult(new PaymentResponse
                {
                    Id = payment.Id,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = payment.PaymentDate,
                    PaymentStatus = payment.PaymentStatus,
                    ReferenceNumber = payment.ReferenceNumber,
                    Notes = payment.Notes,
                }, "Payment status updated successfully.");

            }
            catch (Exception ex)
            {
                return Result<PaymentResponse>.FailureResult("Error updating payment status.", ex.Message);
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
