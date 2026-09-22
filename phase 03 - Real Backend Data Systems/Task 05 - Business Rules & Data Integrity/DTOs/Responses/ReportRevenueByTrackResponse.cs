namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
{
    public class ReportRevenueByTrackResponse
    {
        public int TrackId { get; set; }
        public string TrackTitle { get; set; } = string.Empty;
        public decimal TrackPrice { get; set; }
        public int EnrollmentCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Outstanding { get; set; }
    }
}