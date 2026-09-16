using Task_03___Training_Center_Database_API.Utilities.Interfaces;

namespace Task_03___Training_Center_Database_API.Entities
{
    public class Student : Person , ISoftDelete
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
