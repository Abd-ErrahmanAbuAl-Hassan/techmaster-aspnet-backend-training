using System.ComponentModel.DataAnnotations;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Requests
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