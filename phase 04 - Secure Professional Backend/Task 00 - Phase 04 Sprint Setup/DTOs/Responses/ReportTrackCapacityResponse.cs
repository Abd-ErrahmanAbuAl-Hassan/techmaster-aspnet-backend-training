namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
{
    public class ReportTrackCapacityResponse
    {
        public int TrackId { get; set; }
        public string TrackTitle { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Enrolled { get; set; }
        public int Available => Capacity - Enrolled;
        public double OccupancyPercentage => Capacity > 0 ? (Enrolled * 100.0) / Capacity : 0;
    }
}