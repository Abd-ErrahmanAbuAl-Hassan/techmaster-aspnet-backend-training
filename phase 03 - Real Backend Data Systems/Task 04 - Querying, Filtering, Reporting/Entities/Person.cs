using Task_04_Querying_Filtering_Reporting.Utilities.Interfaces;

namespace Task_04_Querying_Filtering_Reporting.Entities
{
    public abstract class Person : IAudiable
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FullName => FName + " " + LName;
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

    }
}
