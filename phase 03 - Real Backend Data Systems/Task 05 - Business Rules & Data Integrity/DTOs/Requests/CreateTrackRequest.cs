using System.ComponentModel.DataAnnotations;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class CreateTrackRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
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
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}