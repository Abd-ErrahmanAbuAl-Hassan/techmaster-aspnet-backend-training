using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PagedResult<PaymentResponse>>> GetPaymentsAsync(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, PaymentStatus? status = null);
        Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<ApiResponse<PagedResult<PaymentResponse>>> GetEnrollmentPaymentsAsync(int enrollmentId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PaymentResponse>> UpdatePaymentStatusAsync(int id, PaymentStatus status);
    }
}
