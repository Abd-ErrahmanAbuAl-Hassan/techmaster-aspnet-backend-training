namespace Task_03___Training_Center_Database_API.Entities
{
    public class Instructor : Person
    {
        public string Specialization { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
    }
}
