using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Task_04_Querying_Filtering_Reporting.Data;
using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.DTOs.Responses;
using Task_04_Querying_Filtering_Reporting.Entities;
using Task_04_Querying_Filtering_Reporting.Services.Interfaces;
using Task_04_Querying_Filtering_Reporting.Utilities;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Services
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

        public async Task<ApiResponse<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, PaymentStatus? status = null)
        {
            try
            {
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
            try
            {
                var errors = new List<string>();

                if (request.Amount <= 0)
                    errors.Add("Amount must be greater than zero.");

                var enrollment = await _context.Enrollments
                    .Include(e => e.TrainingTrack)
                    .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId);

                if (enrollment == null)
                    errors.Add("Enrollment not found.");

                if (errors.Any())
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = errors
                    };

                var prevPayments = _context.Payments.Where(p => p.EnrollmentId == request.EnrollmentId).AsQueryable();

                if (prevPayments.Any(p => p.PaymentStatus == PaymentStatus.Paid))
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Conflict error",
                        ErrorCode = 409,
                        Errors = new List<string> { "The enrollment is already paid." }
                    };
                decimal totalPaid=0;
                PaymentStatus status = PaymentStatus.Pending;
                if (prevPayments.Any(p => p.PaymentStatus == PaymentStatus.PartiallyPaid))
                {
                    totalPaid = prevPayments.Where(p=>p.PaymentStatus == PaymentStatus.PartiallyPaid)
                       .SumAsync(p => p.Amount).Result;

                }

                if (totalPaid + request.Amount > enrollment!.TrainingTrack!.Price)
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Validation errors.",
                        ErrorCode = 400,
                        Errors = new List<string> { "Payment amount exceeds track price." }
                    };
                else if (totalPaid + request.Amount < enrollment!.TrainingTrack!.Price)
                    status = PaymentStatus.PartiallyPaid;
                else 
                    status = PaymentStatus.Paid;

                    var payment = new Payment
                    {
                        EnrollmentId = request.EnrollmentId,
                        Amount = request.Amount,
                        PaymentMethod = request.PaymentMethod,
                        PaymentDate = DateTime.UtcNow,
                        PaymentStatus =status,
                        ReferenceNumber = $"REF-{request.EnrollmentId}-{request.EnrollmentId+RandomNumberGenerator.GetInt32(0,1000):D3}",
                        Notes = request.Notes
                    };

                _context.Payments.Add(payment);

                if (status == PaymentStatus.Paid)
                    await _enrollmentService.UpdateEnrollmentStatusAsync(enrollment.Id, EnrollmentStatus.Active);

                await _context.SaveChangesAsync();

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
                        ReferenceNumber = payment.ReferenceNumber
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
                (PaymentStatus.PartiallyPaid, PaymentStatus.Paid) => true,
                _ => false
            };
        }
    }
}
