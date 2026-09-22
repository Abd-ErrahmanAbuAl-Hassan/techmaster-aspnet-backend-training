using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
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