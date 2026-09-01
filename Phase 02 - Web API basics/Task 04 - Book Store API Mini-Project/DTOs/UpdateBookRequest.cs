namespace BookStoreApi.DTOs
{
    public class UpdateBookRequest
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
    }
}