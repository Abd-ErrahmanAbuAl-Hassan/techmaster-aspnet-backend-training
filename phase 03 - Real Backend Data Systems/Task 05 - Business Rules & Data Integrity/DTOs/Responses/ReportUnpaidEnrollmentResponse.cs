namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
{
    public class ReportUnpaidEnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public string StudentTitle { get; set; } = string.Empty;
        public string TrackTitle { get; set; } = string.Empty;
        public decimal TrackPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Remaining => TrackPrice - TotalPaid;
        public DateTime EnrollmentDate { get; set; }
    }
}