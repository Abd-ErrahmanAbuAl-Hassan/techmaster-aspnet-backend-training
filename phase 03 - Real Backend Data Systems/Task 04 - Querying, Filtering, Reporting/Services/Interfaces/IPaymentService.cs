using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.DTOs.Responses;
using Task_04_Querying_Filtering_Reporting.Utilities;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, PaymentStatus? status = null);
        Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<ApiResponse<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status);
    }
}
