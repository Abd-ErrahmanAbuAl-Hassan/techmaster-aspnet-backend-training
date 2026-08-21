namespace Task_04_Product_Catalog_with_LINQ.Models
{
    internal class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
        public string SupplierName { get; set; }
        public int StockQuantity { get; set; }

    }

}
