using Task_03_Products_Categories_API.Entities;

namespace Task_03_Products_Categories_API.DTOs
{
    public class CategoryResponse 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductResponse> Products { get; set; }

    }
}
