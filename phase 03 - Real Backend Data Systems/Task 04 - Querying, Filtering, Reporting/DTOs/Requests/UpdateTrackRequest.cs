using System.ComponentModel.DataAnnotations;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Requests
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
