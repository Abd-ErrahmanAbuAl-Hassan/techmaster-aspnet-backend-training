namespace Task_03_Products_Categories_API.DTOs
{
    public class StockReportDto
    {
        public decimal TotalStockValue { get; set; }
        public List<CategoryStockDto> StockValuePerCategory { get; set; }
        public List<LowStockProductDto> LowStockProducts { get; set; }
        public List<StockValueDto> OutOfStockProducts { get; set; }
        public Dictionary<string, int> ProductCountByCategory { get; set; }
        public int TotalProductCount { get; set; }
        public int TotalOutOfStockCount { get; set; }
        public int TotalLowStockCount { get; set; }
    }
}