using Task_07___API_Refactor_Pack.Entities;
using Task_07___API_Refactor_Pack.Utilities.Interfaces;

namespace Task_07___API_Refactor_Pack.Entities
{
    public class Student : Person , ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
