using System.ComponentModel.DataAnnotations;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class CreatePaymentRequest
    {
        [Required]
        public int EnrollmentId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        //[Required]
        //public string ReferenceNumber { get; set; } 
        public string? Notes { get; set; }
    }
}