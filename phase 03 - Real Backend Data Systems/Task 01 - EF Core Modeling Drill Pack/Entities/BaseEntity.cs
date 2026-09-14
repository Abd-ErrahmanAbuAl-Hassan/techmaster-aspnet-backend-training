namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FullName => FName + " " + LName;
        public string Email { get; set; }
    }

    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }

    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
