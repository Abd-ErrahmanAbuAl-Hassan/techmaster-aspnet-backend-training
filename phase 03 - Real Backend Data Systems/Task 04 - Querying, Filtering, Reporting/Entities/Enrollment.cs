using Task_04_Querying_Filtering_Reporting.Utilities.Enums;
using Task_04_Querying_Filtering_Reporting.Utilities.Interfaces;

namespace Task_04_Querying_Filtering_Reporting.Entities
{
    public class Enrollment : IAudiable
    {
        public int Id { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public double? FinalResult { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        public int TrainingTrackId { get; set; }
        public virtual TrainingTrack TrainingTrack { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
