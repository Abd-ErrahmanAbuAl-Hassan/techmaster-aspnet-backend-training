namespace BookStoreApi.DTOs
{
    public class CreateAuthorRequest
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}