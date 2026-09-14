using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public class PaymentSummary
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public decimal TotalRequired { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount => TotalRequired - TotalPaid;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public Enrollment Enrollment { get; set; } = null!;
    }
}
