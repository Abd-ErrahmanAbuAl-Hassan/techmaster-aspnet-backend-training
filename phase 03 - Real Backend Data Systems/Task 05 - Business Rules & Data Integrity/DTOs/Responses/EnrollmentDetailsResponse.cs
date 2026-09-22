using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
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