namespace Task_03___Training_Center_Database_API.Utilities.Interfaces
{
    public interface ISoftDelete
    {
        public DateTime? DeletedAt { get; set; }
    }
}
