using TrainingCenter.Domain.Interfaces;

namespace TrainingCenter.Domain.Entities
{
    public class Student : User , ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
