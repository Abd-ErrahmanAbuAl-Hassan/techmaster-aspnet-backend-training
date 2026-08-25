namespace Task_04_Product_Catalog_with_LINQ.DTOs
{
    internal class CategoryStats
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public decimal Max_Price { get; set; }
        public decimal Min_Price { get; set; }
        public decimal Total_Stock_Value { get; set; }
    }
}
