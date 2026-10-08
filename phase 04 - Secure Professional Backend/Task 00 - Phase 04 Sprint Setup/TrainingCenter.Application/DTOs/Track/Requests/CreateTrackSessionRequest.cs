namespace TrainingCenter.Application.DTOs.Track.Requests
{
    public class CreateTrackSessionRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string MeetingLink { get; set; } = null!;
        public DateTime SessionDate { get; set; }
        public int? InstructorId { get; set; }
    }
}
