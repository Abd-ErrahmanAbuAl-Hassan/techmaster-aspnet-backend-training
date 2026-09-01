namespace BookStoreApi.DTOs
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}