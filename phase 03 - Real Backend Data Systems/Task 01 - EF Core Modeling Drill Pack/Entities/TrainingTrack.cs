namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public class TrainingTrack : IAuditable, ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public int InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; } = null!;


        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}


