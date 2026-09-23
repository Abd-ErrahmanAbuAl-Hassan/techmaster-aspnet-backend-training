using Microsoft.EntityFrameworkCore;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEnrollmentService _enrollmentService;
        public PaymentService(ApplicationDbContext context, IEnrollmentService enrollmentService)
        {
            _context = context;
            _enrollmentService = enrollmentService;
        }

        public async Task<ApiResponse<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, 
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
                    return new ApiResponse<PagedResult<PaymentResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var query = _context.Payments.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(p => p.PaymentDate >= startDate.Value);
                if (endDate.HasValue)
                    query = query.Where(p => p.PaymentDate <= endDate.Value);
                if (status.HasValue)
                    query = query.Where(p => p.PaymentStatus == status.Value);

                var totalCount = await query.CountAsync();
                var payments = await query
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
                    .ToListAsync();

                if (!payments.Any())
                    return new ApiResponse<PagedResult<PaymentResponse>>
                    {
                        Success = false,
                        Message = "No payments are found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                return new ApiResponse<PagedResult<PaymentResponse>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully.",
                    Data = new PagedResult<PaymentResponse>
                    {
                        Items = payments,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<PaymentResponse>>
                {
                    Success = false,
                    Message = "Error retrieving payments.",
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
                if(!Enum.IsDefined(request.PaymentMethod)) errors.Add("Payment method not valid.");

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
        public async Task<ApiResponse<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                var errors = new List<string>();
                if (enrollmentId < 1) errors.Add("Enrollment ID must be positive number.");
                if (pageNumber < 1) errors.Add("Page number must be positive number.");
                if (pageSize < 1) errors.Add("Page size must be positive number.");
                if (pageSize > 50) errors.Add("Page size must be at most 50.");

                if (errors.Any())
                    return new ApiResponse<PagedResult<PaymentResponse>>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == enrollmentId);
                if (enrollment == null)
                    return new ApiResponse<PagedResult<PaymentResponse>>
                    {
                        Success = false,
                        Message = "Enrollment not found.",
                        ErrorCode = 404
                    };

                var query = _context.Payments.Where(p => p.EnrollmentId == enrollmentId);
                var totalCount = await query.CountAsync();
                var payments = await query
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
                    .ToListAsync();

                if (!payments.Any())
                    return new ApiResponse<PagedResult<PaymentResponse>>
                    {
                        Success = false,
                        Message = "No payments are found.",
                        ErrorCode = 404,
                        Errors = new List<string>()
                    };

                return new ApiResponse<PagedResult<PaymentResponse>>
                {
                    Success = true,
                    Message = "Enrollment payments retrieved successfully.",
                    Data = new PagedResult<PaymentResponse>
                    {
                        Items = payments,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<PaymentResponse>>
                {
                    Success = false,
                    Message = "Error retrieving enrollment payments.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        public async Task<ApiResponse<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status)
        {
            try
            {

                var errors = new List<string>();
                if (id < 1) errors.Add("Payment ID must be positive number.");
                if(!Enum.IsDefined(status)) errors.Add("Payment status is invalid.");

                if (errors.Any())
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Validation Errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
                if (payment == null)
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Payment not found.",
                        ErrorCode = 404
                    };

                // Validate status transitions
                if (!IsValidStatusTransition(payment.PaymentStatus, status))
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Invalid status transition.",
                        ErrorCode = 400,
                        Errors = new List<string> { $"Cannot transition from {payment.PaymentStatus} to {status}." }
                    };

                payment.PaymentStatus = status;
                await _context.SaveChangesAsync();

                return new ApiResponse<PaymentResponse>
                {
                    Success = true,
                    Message = "Payment status updated successfully.",
                    Data = new PaymentResponse
                    {
                        Id = payment.Id,
                        Amount = payment.Amount,
                        PaymentMethod = payment.PaymentMethod,
                        PaymentDate = payment.PaymentDate,
                        PaymentStatus = payment.PaymentStatus,
                        ReferenceNumber = payment.ReferenceNumber,
                        Notes = payment.Notes,
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaymentResponse>
                {
                    Success = false,
                    Message = "Error updating payment status.",
                    ErrorCode = 500,
                    Errors = new List<string> { ex.Message }
                };
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
