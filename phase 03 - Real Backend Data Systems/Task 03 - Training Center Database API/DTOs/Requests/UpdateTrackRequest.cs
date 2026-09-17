using System.ComponentModel.DataAnnotations;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.DTOs.Requests
{
    public class UpdateTrackRequest
    {
        [Required]
        public int InstructorId { get; set; }
        public string? Name { get; set; } 
        public string? Description { get; set; } 
        public int? Capacity { get; set; }
        public TrackLevel? Level { get; set; }
        public decimal? Price { get; set; }
    }
}
