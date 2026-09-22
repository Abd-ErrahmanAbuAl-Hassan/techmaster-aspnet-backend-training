namespace Task_05_Business_Rules_Data_Integrity.Utilities.Interfaces
{
    public interface IAudiable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
