namespace TrainingCenter.Domain.Entities
{
    public class TrackSession
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MeetingLink { get; set; }
        public DateTime? CompletedAt  { get; set; } // this will be treated as the session termination 
        public DateTime SessionDate { get; set; }
        public int CreatedByInstructorId { get; set; }
        public virtual Instructor Instructor { get; set; } = null!;

        public int TrackId { get; set; }
        public virtual TrainingTrack Track { get; set; } = null!;
    }
}
