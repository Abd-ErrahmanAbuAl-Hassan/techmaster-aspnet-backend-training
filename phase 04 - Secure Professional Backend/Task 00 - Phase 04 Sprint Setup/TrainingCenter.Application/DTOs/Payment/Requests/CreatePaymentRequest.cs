using System.ComponentModel.DataAnnotations;
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Payment.Requests
{
    public class CreatePaymentRequest
    {
        [Required]
        public int EnrollmentId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        public string Notes { get; set; }
    }
}