using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class PaymentStatusUpdateRequest
    {
        public PaymentStatus Status { get; set; }
    }
}