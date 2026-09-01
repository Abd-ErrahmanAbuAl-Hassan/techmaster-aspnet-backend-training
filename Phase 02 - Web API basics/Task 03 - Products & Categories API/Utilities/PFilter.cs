namespace Task_03_Products_Categories_API.Utilities
{
    public class PFilter
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? LowStockThreshold { get; set; }
        public bool? IsAvilable { get; set; }
    }
}
