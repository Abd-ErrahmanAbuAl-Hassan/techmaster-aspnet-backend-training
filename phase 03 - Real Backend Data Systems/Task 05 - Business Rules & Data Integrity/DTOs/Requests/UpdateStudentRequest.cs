namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
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