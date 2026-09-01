namespace Task_03_Products_Categories_API.DTOs
{
    public class LowStockProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public int CurrentStock { get; set; }
        public int LowStockThreshold { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }
    }
}