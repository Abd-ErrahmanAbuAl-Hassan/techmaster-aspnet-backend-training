namespace Task_07___API_Refactor_Pack.Entities
{
    public class Instructor : Person
    {
        public string Specialization { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
    }
}
