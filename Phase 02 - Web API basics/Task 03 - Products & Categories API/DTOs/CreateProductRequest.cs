using System.ComponentModel.DataAnnotations;

namespace Task_03_Products_Categories_API.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public string SupplierName { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }
}
