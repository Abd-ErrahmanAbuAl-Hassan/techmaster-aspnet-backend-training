using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }

        public int EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; } = null!;
    }
}
