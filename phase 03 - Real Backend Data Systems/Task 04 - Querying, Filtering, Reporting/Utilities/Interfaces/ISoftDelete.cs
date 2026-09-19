namespace Task_04_Querying_Filtering_Reporting.Utilities.Interfaces
{
    public interface ISoftDelete
    {
        public DateTime? DeletedAt { get; set; }
    }
}
