using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Application.DTOs.Enrollment.Requests
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