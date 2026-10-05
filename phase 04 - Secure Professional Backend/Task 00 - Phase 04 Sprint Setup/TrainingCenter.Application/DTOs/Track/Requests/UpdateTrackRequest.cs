using System.ComponentModel.DataAnnotations;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Track.Requests
{
    public class UpdateTrackRequest
    {
        [Required]
        public int InstructorId { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } 
        public int? Capacity { get; set; }
        public TrackLevel? Level { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
