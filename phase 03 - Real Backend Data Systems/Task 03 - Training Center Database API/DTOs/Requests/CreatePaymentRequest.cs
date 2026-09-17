using System.ComponentModel.DataAnnotations;
using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.DTOs.Requests
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