using TrainingCenter.Application.DTOs.Enrollment.Requests;
using TrainingCenter.Application.DTOs.Enrollment.Responses;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Results;

namespace TrainingCenter.Application.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<Result<PagedResult<EnrollmentDetailsResponse>>> GetEnrollmentsAsync(int pageNumber = 1, int pageSize = 10, EnrollmentStatus? status = null, int? trackId = null, int? studentId = null, PaymentStatus? paymentStatus = null);
        Task<Result<EnrollmentDetailsResponse>> GetEnrollmentByIdAsync(int id);
        Task<Result<EnrollmentDetailsResponse>> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        Task<Result<EnrollmentDetailsResponse>> UpdateEnrollmentStatusAsync(int id, EnrollmentStatus status);
        Task<Result<PagedResult<EnrollmentSummaryResponse>>> GetStudentEnrollmentsAsync(int studentId, int pageNumber = 1, int pageSize = 10);
        Task<Result<PagedResult<TrackEnrollmentStudents>>> GetTrackStudentsAsync(int trackId, int pageNumber = 1, int pageSize = 10);
    }
}
