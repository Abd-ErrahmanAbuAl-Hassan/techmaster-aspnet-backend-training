using Task_03___Training_Center_Database_API.DTOs.Requests;
using Task_03___Training_Center_Database_API.DTOs.Responses;
using Task_03___Training_Center_Database_API.Utilities;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10, EnrollmentStatus? status = null, int? trackId = null, int? studentId = null, PaymentStatus? paymentStatus = null);
        Task<ApiResponse<EnrollmentDetailsResponse>> GetEnrollmentByIdAsync(int id);
        Task<ApiResponse<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        Task<ApiResponse<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status);
        Task<ApiResponse<PagedResult<EnrollmentSummaryResponse>>> GetStudentEnrollmentsAsync(int studentId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<StudentListItemResponse>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10);
    }
}
