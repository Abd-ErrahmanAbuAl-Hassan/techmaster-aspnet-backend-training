namespace Task_04_Querying_Filtering_Reporting.Entities
{
    public class Instructor : Person
    {
        public string Specialization { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
    }
}
