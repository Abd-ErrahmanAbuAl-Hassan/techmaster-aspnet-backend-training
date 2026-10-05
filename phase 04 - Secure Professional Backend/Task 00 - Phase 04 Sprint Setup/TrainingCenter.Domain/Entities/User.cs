using TrainingCenter.Domain.Interfaces;

namespace TrainingCenter.Domain.Entities
{
    public abstract class User : IAudiable
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FullName => FName + " " + LName;
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHashed { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

    }
}
