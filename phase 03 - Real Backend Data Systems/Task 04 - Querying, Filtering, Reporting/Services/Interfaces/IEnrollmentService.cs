using Task_04_Querying_Filtering_Reporting.DTOs.Requests;
using Task_04_Querying_Filtering_Reporting.DTOs.Responses;
using Task_04_Querying_Filtering_Reporting.Utilities;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10, EnrollmentStatus? status = null, int? trackId = null, int? studentId = null, PaymentStatus? paymentStatus = null);
        Task<ApiResponse<EnrollmentDetailsResponse>> GetEnrollmentByIdAsync(int id);
        Task<ApiResponse<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        Task<ApiResponse<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status);
        Task<ApiResponse<PagedResult<EnrollmentSummaryResponse>>> GetStudentEnrollmentsAsync(int studentId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<TrackEnrollmentStudents>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10);
    }
}
