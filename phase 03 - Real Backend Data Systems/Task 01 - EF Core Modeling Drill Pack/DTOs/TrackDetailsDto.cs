namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InstructorName { get; set; }

    }
    public class TrackDetailsDto : TrackDto
    {
        public int EnrolledStudentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class TrackStudents
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; }
        public PaginationResult<StudentListItemDto> Students { get; set; }
    }
}
