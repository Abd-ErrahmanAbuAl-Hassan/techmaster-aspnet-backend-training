using Task_03_Products_Categories_API.Entities;

namespace Task_03_Products_Categories_API.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public string SupplierName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // This is required when mapping to Db because it present the relationship between Product and Category in Db
        //public Guid CategoryId { get; set; } 

        // In this project we do not need it. I just use the concept of OOP (composition) to say the product has a relation with category
        //Navigation property
        public Category Category { get; set; }
    }
}
