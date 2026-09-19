using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
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