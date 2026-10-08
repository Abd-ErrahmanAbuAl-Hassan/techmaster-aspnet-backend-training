namespace TrainingCenter.Application.DTOs.Track.Requests
{
    public class UpdateSessionRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? MeetingLink { get; set; }
        public DateTime? SessionDate { get; set; }
        public int InstructorId { get; set; }
    }
}
