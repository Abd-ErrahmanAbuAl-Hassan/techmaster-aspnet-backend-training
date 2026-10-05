using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Payment.Requests
{
    public class PaymentStatusUpdateRequest
    {
        public PaymentStatus Status { get; set; }
    }
}