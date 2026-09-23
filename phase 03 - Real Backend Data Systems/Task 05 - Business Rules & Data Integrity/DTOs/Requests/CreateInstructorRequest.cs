using System.ComponentModel.DataAnnotations;

namespace Task_05_Business_Rules_Data_Integrity.DTOs.Requests
{
    public class CreateInstructorRequest: CreateStudentRequest
    {
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Bio { get; set; }
    }
}