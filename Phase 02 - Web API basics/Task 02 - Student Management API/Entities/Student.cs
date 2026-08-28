namespace Task_02___Student_Management_API.Entities
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FName { get; set; }
        public string LName { get; set; }
        public string FullName => FName + " " + LName;
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TrackName { get; set; }
        public string LinkedInURL { get; set; }
        public string GithubURL { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }

    }
}
