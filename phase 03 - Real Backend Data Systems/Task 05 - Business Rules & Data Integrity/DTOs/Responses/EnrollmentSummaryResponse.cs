using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
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