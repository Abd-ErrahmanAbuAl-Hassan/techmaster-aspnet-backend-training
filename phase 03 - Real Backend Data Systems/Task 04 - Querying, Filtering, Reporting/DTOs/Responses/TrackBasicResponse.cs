using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.DTOs.Responses
{
    public class TrackBasicResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public decimal Price { get; set; }
    }
}