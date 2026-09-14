using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class EnrollmentDTO
    {
        public string StudentName { get; set; }
        public string Track { get; set; }
        public EnrollmentStatus Status { get; set; } 
        public DateTime EnrollmentDate { get; set; } 
        public double? FinalGrade { get; set; }
    }
}
