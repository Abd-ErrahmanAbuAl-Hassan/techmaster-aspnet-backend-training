namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public class Instructor : BaseEntity, IAuditable
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
    }
}
