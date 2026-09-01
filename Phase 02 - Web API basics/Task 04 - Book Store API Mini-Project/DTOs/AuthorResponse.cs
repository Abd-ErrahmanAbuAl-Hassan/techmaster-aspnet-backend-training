namespace BookStoreApi.DTOs
{
    public class AuthorResponse
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}