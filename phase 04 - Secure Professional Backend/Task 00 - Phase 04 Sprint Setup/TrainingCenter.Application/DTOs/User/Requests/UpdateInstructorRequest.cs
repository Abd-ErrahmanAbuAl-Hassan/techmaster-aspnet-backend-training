namespace TrainingCenter.Application.DTOs.User.Requests
{
    public class UpdateInstructorRequest
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
    }
}