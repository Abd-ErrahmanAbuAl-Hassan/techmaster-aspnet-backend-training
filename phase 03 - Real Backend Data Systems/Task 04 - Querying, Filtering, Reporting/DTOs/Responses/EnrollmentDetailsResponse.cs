using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class EnrollmentDetailsResponse
    {
        public int Id { get; set; }
        public StudentListItemResponse Student { get; set; } = new();
        public TrackBasicResponse Track { get; set; } = new();
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public double? FinalGrade { get; set; }
        public decimal TotalPaid { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<PaymentResponse> Payments { get; set; } = new();
    }
}