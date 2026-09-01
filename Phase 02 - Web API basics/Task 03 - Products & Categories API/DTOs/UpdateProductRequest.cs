using System.ComponentModel.DataAnnotations;

namespace Task_03_Products_Categories_API.DTOs
{
    public class UpdateProductRequest 
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? SupplierName { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
