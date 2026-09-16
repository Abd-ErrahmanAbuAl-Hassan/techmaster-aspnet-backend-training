using Task_03___Training_Center_Database_API.Utilities.Enums;
using Task_03___Training_Center_Database_API.Utilities.Interfaces;

namespace Task_03___Training_Center_Database_API.Entities
{
    public class Enrollment : IAudiable
    {
        public int Id { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public decimal? FinalResult { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        public int TrainingTrackId { get; set; }
        public virtual TrainingTrack TrainingTrack { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
