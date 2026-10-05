using TrainingCenter.Application.DTOs.Payment.Requests;
using TrainingCenter.Application.DTOs.Payment.Responses;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, PaymentStatus? status = null);
        Task<Result<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<Result<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10);
        Task<Result<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status);
    }
}
