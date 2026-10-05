using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Application.DTOs.User.Requests
{
    public class CreateInstructorRequest: CreateStudentRequest
    {
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Bio { get; set; }
    }
}