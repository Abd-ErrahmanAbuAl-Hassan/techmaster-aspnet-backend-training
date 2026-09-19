using Task_04_Querying_Filtering_Reporting.Utilities.Enums;
using Task_04_Querying_Filtering_Reporting.Utilities.Interfaces;

namespace Task_04_Querying_Filtering_Reporting.Entities
{
    public class TrainingTrack : IAudiable , ISoftDelete
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TrackLevel Level { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public TrackStatus Status { get; set; }
        public bool IsActive { get; set; }
        public int InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; } = null!;

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    }
}
