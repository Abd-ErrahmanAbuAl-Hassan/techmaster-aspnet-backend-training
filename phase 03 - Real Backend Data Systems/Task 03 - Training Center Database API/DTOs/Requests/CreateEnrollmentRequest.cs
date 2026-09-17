using System.ComponentModel.DataAnnotations;

namespace Task_03___Training_Center_Database_API.DTOs.Requests
{
    public class CreateEnrollmentRequest
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int TrainingTrackId { get; set; }
    }
}