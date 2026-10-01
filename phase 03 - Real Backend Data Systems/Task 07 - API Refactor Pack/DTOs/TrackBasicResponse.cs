using Task_07___API_Refactor_Pack.Utilities.Enums;

namespace Task_07___API_Refactor_Pack.DTOs
{
    public class TrackBasicResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public decimal Price { get; set; }
    }
}