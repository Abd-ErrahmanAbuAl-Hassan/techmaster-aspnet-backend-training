namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class ReportTrackAvailableSeatsResponse
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int ActiveEnrollments { get; set; }
        public int RemainingSeats => Capacity - ActiveEnrollments >= 0 ? Capacity - ActiveEnrollments : 0;
    }
}