using System.ComponentModel.DataAnnotations;

namespace Task_02___Student_Management_API.DTOs
{
    public class UpdateStudentRequest
    {
        [StringLength(20, MinimumLength = 3)]
        public string? FName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string? LName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [RegularExpression(@"^01[0125]\d{8}$]", ErrorMessage = "Only allow EGY phone numbers.")]
        public string? PhoneNumber { get; set; }

        public string? TrackName { get; set; }
        public string? LinkedInURL { get; set; }
        public string? GithubURL { get; set; }
    }
}
