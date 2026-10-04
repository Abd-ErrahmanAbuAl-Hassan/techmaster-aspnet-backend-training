using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;
using Task_05_Business_Rules_Data_Integrity.DTOs.Responses;
using Task_05_Business_Rules_Data_Integrity.Utilities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Services.Interfaces
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
