namespace Task_03___Training_Center_Database_API.DTOs.Responses
{
    public class InstructorBasicResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}