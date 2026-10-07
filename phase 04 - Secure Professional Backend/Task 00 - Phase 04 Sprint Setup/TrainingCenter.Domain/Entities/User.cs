using TrainingCenter.Domain.Enums;
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
        public string PasswordHashed { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
