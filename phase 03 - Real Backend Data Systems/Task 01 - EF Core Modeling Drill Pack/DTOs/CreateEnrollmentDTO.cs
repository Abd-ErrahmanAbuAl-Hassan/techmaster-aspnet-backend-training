using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class CreateEnrollmentDTO
    {
        public int StudentId { get; set; }
        public int TrainingTrackId { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public double? FinalGrade { get; set; }
    }
}