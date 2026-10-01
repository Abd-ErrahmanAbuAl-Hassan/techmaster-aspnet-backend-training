using System.ComponentModel.DataAnnotations;

namespace Task_06___API_Standards___Refactor_Pack.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
