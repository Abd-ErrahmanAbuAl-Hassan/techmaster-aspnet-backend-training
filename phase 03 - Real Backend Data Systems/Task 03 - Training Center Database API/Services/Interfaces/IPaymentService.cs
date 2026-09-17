using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, PaymentStatus? status = null);
        Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<ApiResponse<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status);
    }
}
