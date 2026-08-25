namespace Task_04_Product_Catalog_with_LINQ.Models
{
    internal class ProductFilter
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Category { get; set; }
        public bool? Available { get; set; }
    }
}
