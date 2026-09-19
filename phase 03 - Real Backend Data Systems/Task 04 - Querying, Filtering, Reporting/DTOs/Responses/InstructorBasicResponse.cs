namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class InstructorBasicResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class InstructorWorkLoadResponse : InstructorBasicResponse
    {
        public int TrackCount { get; set; }
        public int ActiveStudents { get; set; }

    }
}