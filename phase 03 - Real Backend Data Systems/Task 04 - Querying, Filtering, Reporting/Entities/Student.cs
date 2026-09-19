using Task_04_Querying_Filtering_Reporting.Utilities.Interfaces;

namespace Task_04_Querying_Filtering_Reporting.Entities
{
    public class Student : Person , ISoftDelete
    {
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
