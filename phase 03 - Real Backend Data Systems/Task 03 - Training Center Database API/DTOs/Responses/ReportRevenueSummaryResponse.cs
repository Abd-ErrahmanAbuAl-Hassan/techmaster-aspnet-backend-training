namespace Task_03___Training_Center_Database_API.DTOs.Responses
{
    public class ReportRevenueSummaryResponse
    {
        public decimal TotalRevenue { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PartiallyPaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public int TotalPayments { get; set; }
        public int PaidCount { get; set; }
    }
}