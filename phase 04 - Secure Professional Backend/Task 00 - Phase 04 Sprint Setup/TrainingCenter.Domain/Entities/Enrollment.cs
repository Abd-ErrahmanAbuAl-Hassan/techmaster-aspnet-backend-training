using TrainingCenter.Domain.Enums;
using TrainingCenter.Domain.Interfaces;

namespace TrainingCenter.Domain.Entities
{
    public class Enrollment : IAudiable
    {
        public int Id { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public decimal? FinalGrade { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        public int TrainingTrackId { get; set; }
        public virtual TrainingTrack TrainingTrack { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = [];

    }
}
