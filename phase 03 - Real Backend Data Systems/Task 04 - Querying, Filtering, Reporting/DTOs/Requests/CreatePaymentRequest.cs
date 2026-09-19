using System.ComponentModel.DataAnnotations;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Requests
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