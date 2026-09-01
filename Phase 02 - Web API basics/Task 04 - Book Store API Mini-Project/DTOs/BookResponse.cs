namespace BookStoreApi.DTOs
{
    public class BookResponse
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}