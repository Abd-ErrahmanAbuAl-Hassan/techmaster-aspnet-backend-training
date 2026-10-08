using TrainingCenter.Application.DTOs.User.Responses;

namespace TrainingCenter.Application.DTOs.Track.Responses
{
    public class BasicTrackSessionResponse
    {
        public int SessionId { get; set; }
        public string Title { get; set; } = null!;
        public string MeetingLink { get; set; } = null!;
        public DateTime SessionDate { get; set; } 
    }
    public class DetailedTrackSessionResponse
    {
        public int SessionId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string MeetingLink { get; set; } = null!;
        public DateTime SessionDate { get; set; } 
        public InstructorBasicResponse Instructor {  get; set; } = new ();
    }
}
