namespace Task_03_Products_Categories_API.DTOs
{
    public class CategoryStockDto
    {
        public string CategoryName { get; set; }
        public decimal TotalValue { get; set; }
        public int ProductCount { get; set; }
    }
}