using System.ComponentModel.DataAnnotations;
using Task_07___API_Refactor_Pack.Utilities.Enums;

namespace Task_07___API_Refactor_Pack.DTOs
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