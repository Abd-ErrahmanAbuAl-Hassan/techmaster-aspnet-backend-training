using System.ComponentModel.DataAnnotations;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.DTOs.Requests
{
    public class CreateTrackRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int InstructorId { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public TrackLevel Level { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}