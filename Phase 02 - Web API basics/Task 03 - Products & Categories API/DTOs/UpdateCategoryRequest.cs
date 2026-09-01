using System.ComponentModel.DataAnnotations;

namespace Task_03_Products_Categories_API.DTOs
{
    public class UpdateCategoryRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? NewStatus { get; set; }
    }
}
