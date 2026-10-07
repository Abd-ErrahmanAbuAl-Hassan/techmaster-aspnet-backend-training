using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Auth.Response
{
    public class CurrentUserResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
    }
}
