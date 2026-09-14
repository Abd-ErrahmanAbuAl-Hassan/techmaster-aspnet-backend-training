using System.ComponentModel.DataAnnotations;

namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class CreateTrackRequest
    {
        [Required]
        public string Name { get; set; } 
        [Required]
        public string Description { get; set; } 
    }
    public class UpdateTrackRequest
    {
        [Required]
        public int InstructorId { get; set; }
        public string? Name { get; set; } 
        public string? Description { get; set; } 
    }
}
