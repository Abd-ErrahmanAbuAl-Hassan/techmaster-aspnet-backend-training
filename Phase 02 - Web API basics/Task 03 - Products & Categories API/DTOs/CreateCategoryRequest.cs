using System.ComponentModel.DataAnnotations;

namespace Task_03_Products_Categories_API.DTOs
{
    public class CreateCategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
