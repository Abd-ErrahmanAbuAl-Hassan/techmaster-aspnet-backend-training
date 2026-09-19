namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class ReportUnpaidEnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public decimal TrackPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Remaining => TrackPrice - TotalPaid;
        public DateTime EnrollmentDate { get; set; }
    }
}