namespace Task_03_Products_Categories_API.DTOs
{
    public class StockValueDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
    }
}