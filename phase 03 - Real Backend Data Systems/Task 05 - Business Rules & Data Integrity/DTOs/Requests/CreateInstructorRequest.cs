using System.ComponentModel.DataAnnotations;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class CreateInstructorRequest
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; } 
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Bio { get; set; }
    }
}