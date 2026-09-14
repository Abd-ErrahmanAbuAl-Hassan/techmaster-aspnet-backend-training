namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public class Student : BaseEntity, ISoftDeletable, IAuditable
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual StudentProfile StudentProfile { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
