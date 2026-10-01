using Task_07___API_Refactor_Pack.Utilities.Enums;

namespace Task_07___API_Refactor_Pack.DTOs
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