using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.DTOs.Responses
{
    public class EnrollmentSummaryResponse
    {
        public int Id { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}