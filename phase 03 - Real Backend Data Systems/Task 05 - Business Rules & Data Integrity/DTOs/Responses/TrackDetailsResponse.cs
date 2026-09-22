using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
{
    public class TrackDetailsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public TrackStatus Status { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public int AvailableSeats => Capacity - EnrolledCount;
        public InstructorBasicResponse Instructor { get; set; } = new();
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