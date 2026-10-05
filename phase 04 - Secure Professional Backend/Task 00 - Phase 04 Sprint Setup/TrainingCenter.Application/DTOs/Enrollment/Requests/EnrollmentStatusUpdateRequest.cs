using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Enrollment.Requests
{
    public class EnrollmentStatusUpdateRequest
    {
        public EnrollmentStatus Status { get; set; }
    }
}