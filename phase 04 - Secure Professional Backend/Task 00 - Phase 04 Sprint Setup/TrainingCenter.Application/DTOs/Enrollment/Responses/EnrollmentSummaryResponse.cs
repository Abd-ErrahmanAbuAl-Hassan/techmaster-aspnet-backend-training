using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Enrollment.Responses
{
    public class EnrollmentSummaryResponse
    {
        public int Id { get; set; }
        public string TrackTitle { get; set; } = string.Empty;
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}