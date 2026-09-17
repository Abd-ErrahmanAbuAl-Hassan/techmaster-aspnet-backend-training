using Task_03___Training_Center_Database_API.Utilities.Enums;

namespace Task_03___Training_Center_Database_API.DTOs.Responses
{
    public class TrackBasicResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public decimal Price { get; set; }
    }
}