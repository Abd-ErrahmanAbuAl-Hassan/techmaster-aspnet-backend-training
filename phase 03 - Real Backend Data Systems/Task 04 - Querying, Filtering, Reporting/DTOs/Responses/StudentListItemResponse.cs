using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class StudentListItemResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class TrackEnrollmentStudents : StudentListItemResponse
    {
        public EnrollmentStatus EnrollmentStatus { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
    public class StudentsWithoutPayment
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PaymentResponse> Payments { get; set; } = new();
    }
}