namespace Task_03___Training_Center_Database_API.DTOs.Responses
{
    public class ReportDashboardResponse
    {
        public int TotalStudents { get; set; }
        public int ActiveEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public int TotalTracks { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal UnpaidAmount { get; set; }
    }
}