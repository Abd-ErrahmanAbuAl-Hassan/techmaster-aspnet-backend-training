namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class StudentDetailsResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int EnrollmentCount { get; set; }
        public List<EnrollmentSummaryResponse> Enrollments { get; set; } = new();
    }
}