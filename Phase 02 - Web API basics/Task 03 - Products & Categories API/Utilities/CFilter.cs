namespace Task_03_Products_Categories_API.Utilities
{
    public class CFilter
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
