using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class CreatePaymentSummaryDTO
    {
        public decimal TotalRequired { get; set; }
        public decimal TotalPaid { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    }
}