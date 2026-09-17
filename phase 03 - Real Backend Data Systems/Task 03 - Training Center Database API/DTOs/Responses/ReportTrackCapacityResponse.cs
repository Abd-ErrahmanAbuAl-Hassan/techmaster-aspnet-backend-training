namespace Task_03___Training_Center_Database_API.DTOs.Responses
{
    public class ReportTrackCapacityResponse
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Enrolled { get; set; }
        public int Available => Capacity - Enrolled;
        public double OccupancyPercentage => Capacity > 0 ? (Enrolled * 100.0) / Capacity : 0;
    }
}