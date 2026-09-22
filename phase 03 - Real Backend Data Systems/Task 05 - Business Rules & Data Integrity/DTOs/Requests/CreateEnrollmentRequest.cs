using System.ComponentModel.DataAnnotations;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class CreateEnrollmentRequest
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int TrainingTrackId { get; set; }
        [Required]
        public bool AllowInactiveStudent { get; set; }
    }
}