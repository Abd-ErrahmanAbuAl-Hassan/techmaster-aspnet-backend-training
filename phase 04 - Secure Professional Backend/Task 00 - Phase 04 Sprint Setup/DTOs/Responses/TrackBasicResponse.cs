using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Responses
{
    public class TrackBasicResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TrackLevel Level { get; set; }
        public decimal Price { get; set; }
    }
}