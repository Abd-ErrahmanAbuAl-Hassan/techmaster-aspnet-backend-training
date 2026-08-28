namespace Task_02___Student_Management_API.DTOs
{
    public class StudentStatsResponse
    {
        public int TotalStudents { get; set; } = 0;
        public int ActiveStudents { get; set; } = 0;
        public int InActiveStudents { get; set; } = 0;
        public List<Dictionary<string,int>> CountByTrack { get; set; } = new List<Dictionary<string,int>>();
    }
}
