using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public class Enrollment 
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int TrainingTrackId { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public double? FinalGrade { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual TrainingTrack TrainingTrack { get; set; } = null!;

        public PaymentSummary? PaymentSummary { get; set; }
    }
}
