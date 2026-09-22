namespace Task_05_Business_Rules_Data_Integrity.Entities
{
    public class Instructor : Person
    {
        public string Specialization { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
    }
}
