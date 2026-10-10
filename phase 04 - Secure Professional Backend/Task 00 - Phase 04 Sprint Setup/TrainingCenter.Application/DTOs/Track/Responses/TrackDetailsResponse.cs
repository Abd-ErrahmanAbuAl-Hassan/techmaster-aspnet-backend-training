using TrainingCenter.Application.DTOs.User.Responses;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Track.Responses
{
    public class TrackDetailsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public TrackStatus Status { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public int AvailableSeats => Capacity - EnrolledCount;
        public InstructorBasicResponse Instructor { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class TrackMiniDetailsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public TrackStatus Status { get; set; }
        public int EnrolledCount { get; set; }

    }
}