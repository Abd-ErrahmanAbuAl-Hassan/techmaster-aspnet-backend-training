namespace BookStoreApi.DTOs
{
    public class SummaryResponse
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int OutOfStockBooks { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public Dictionary<string, int> BooksByCategory { get; set; }
        public Dictionary<string, int> BooksByAuthor { get; set; }

        public SummaryResponse()
        {
            BooksByCategory = new Dictionary<string, int>();
            BooksByAuthor = new Dictionary<string, int>();
        }
    }
}