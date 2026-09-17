namespace Task_03___Training_Center_Database_API.DTOs.Requests
{
    public class UpdateStudentRequest
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}