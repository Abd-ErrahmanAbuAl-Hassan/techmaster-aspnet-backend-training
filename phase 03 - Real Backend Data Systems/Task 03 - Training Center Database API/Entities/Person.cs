using Task_03___Training_Center_Database_API.Utilities.Interfaces;

namespace Task_03___Training_Center_Database_API.Entities
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
