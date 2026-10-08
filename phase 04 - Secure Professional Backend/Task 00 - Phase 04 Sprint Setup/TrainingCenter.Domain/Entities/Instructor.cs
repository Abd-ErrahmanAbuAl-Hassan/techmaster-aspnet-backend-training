namespace TrainingCenter.Domain.Entities
{
    public class Instructor : User
    {
        public string Specialization { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
        public virtual ICollection<TrackSession> Sessions { get; set; } = new List<TrackSession>();
    }
}
