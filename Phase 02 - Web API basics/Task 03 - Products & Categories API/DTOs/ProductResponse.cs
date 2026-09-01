using Task_03_Products_Categories_API.Entities;

namespace Task_03_Products_Categories_API.DTOs
{
    public class ProductResponse 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public string SupplierName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Category { get; set; }
    }
}
