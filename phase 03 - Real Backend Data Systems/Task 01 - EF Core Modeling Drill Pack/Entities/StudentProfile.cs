namespace Task_01___EF_Core_Modeling_Drill_Pack.Entities
{
    public class StudentProfile 
    {
        public string SSN { get; set; }
        public string Address { get; set; }
        public string EmergencyPhone { get; set; }
        public DateTime BirthOfDate { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
