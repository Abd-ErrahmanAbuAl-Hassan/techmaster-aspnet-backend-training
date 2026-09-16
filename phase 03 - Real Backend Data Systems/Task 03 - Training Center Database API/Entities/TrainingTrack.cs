using Task_03___Training_Center_Database_API.Utilities.Enums;
using Task_03___Training_Center_Database_API.Utilities.Interfaces;

namespace Task_03___Training_Center_Database_API.Entities
{
    public class TrainingTrack : IAudiable , ISoftDelete
    {
        public int Id { get; set; }
        public int Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public TrackLevel Level { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public TrackStatus Status { get; set; }

        public int InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; } = null!;

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    }
}
