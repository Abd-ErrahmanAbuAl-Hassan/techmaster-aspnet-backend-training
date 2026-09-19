using System.ComponentModel.DataAnnotations;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Requests
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