namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class ReportRevenueByTrackResponse
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public decimal TrackPrice { get; set; }
        public int EnrollmentCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Outstanding { get; set; }
    }
}