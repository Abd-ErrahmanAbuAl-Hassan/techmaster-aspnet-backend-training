using System.ComponentModel.DataAnnotations;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class UpdateTrackRequest
    {
        [Required]
        public int InstructorId { get; set; }
        public string? Title { get; set; } 
        public string? Description { get; set; } 
        public int? Capacity { get; set; }
        public TrackLevel? Level { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
