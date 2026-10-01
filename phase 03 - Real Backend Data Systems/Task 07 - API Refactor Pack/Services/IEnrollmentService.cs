using Task_07___API_Refactor_Pack.DTOs;
using Task_07___API_Refactor_Pack.Utilities;

namespace Task_07___API_Refactor_Pack.Services
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<ApiResponse<string>> DeleteEnrollmentAsync(int enrollmentId);
    }
}
