namespace TrainingCenter.Application.DTOs.Track.Responses
{
    public class TrackProgressResponse
    {
        public string TrackTitle { get; set; } = null!;
        public int EnrollmentCount { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
