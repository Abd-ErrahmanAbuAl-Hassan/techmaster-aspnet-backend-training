using System.ComponentModel.DataAnnotations;

namespace Task_07___API_Refactor_Pack.DTOs
{
    public class CreateEnrollmentRequest
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int TrainingTrackId { get; set; }
    }
}