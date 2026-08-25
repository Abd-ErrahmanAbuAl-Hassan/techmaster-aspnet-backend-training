using System.Security.Principal;

namespace Task_04_Product_Catalog_with_LINQ.DTOs
{
    internal class SupplierReport
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int Stock_Value { get; set; }
        public decimal Avg_Price { get; set; }
    }
}
