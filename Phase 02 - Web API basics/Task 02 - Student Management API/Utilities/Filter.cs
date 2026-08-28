namespace Task_02___Student_Management_API.Utilities
{
    public class Filter
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public string? TrackName { get; set; }
        public bool? IsActive { get; set; }
    }
}
