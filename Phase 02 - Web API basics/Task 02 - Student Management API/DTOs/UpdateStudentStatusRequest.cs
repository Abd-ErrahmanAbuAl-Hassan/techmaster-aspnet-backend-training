using System.ComponentModel.DataAnnotations;

namespace Task_02___Student_Management_API.DTOs
{
    public class UpdateStudentStatusRequest
    {
        [Required]
        public bool NewStatus { get; set; }
    }
}
