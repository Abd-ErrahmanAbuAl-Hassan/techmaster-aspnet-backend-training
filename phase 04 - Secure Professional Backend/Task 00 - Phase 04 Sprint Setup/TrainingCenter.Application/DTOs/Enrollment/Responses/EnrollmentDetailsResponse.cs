using TrainingCenter.Application.DTOs.Payment.Responses;
using TrainingCenter.Application.DTOs.Track.Responses;
using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Enrollment.Responses
{
    public class EnrollmentDetailsResponse
    {
        public int Id { get; set; }
        public StudentListItemResponse Student { get; set; } = new();
        public TrackBasicResponse Track { get; set; } = new();
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal? FinalGrade { get; set; }
        public decimal TotalPaid { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<PaymentResponse> Payments { get; set; } = new();
    }
}