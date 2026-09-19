using System.ComponentModel.DataAnnotations;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Requests
{
    public class CreateEnrollmentRequest
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int TrainingTrackId { get; set; }
    }
}