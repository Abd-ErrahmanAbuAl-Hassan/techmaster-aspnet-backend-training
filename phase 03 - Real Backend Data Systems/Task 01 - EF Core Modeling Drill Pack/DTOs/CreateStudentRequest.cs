using System.ComponentModel.DataAnnotations;

namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class CreateStudentRequest
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string SSN { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string EmergencyPhone { get; set; }
        [Required]
        public DateTime BirthOfDate { get; set; }
    }
    public class UpdateStudentRequest
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Email { get; set; }
        public string? SSN { get; set; }
        public string? Address { get; set; }
        public string? EmergencyPhone { get; set; }
        public DateTime? BirthOfDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
