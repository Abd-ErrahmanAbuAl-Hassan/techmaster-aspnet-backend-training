using System.ComponentModel.DataAnnotations;

namespace Task_03___Training_Center_Database_API.DTOs.Requests
{
    public class CreateStudentRequest
    {
        [Required]
        public string FName { get; set; } = string.Empty;
        [Required]
        public string LName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}