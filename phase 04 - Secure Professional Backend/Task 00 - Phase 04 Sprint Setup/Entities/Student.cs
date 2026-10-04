using Task_05_Business_Rules_Data_Integrity.Utilities.Interfaces;

namespace Task_05_Business_Rules_Data_Integrity.Entities
{
    public class Student : Person , ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
