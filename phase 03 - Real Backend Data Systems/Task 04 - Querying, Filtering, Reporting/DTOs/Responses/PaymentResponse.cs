using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}