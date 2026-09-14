namespace Task_01___EF_Core_Modeling_Drill_Pack.DTOs
{
    public class InstructorDTO
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}