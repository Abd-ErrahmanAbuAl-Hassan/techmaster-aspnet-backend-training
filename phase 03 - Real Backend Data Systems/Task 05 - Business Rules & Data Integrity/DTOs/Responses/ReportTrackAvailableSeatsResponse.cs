namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
{
    public class ReportTrackAvailableSeatsResponse
    {
        public int TrackId { get; set; }
        public string TrackTitle { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int ActiveEnrollments { get; set; }
        public int RemainingSeats => Capacity - ActiveEnrollments >= 0 ? Capacity - ActiveEnrollments : 0;
    }
}