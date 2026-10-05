using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Track.Responses
{
    public class TrackBasicResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public decimal Price { get; set; }
    }
}