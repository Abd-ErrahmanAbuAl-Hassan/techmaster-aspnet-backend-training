using System.ComponentModel.DataAnnotations;

namespace Task_01___REST_Routing_Drill_Pack.DTOs
{
    public class CreateNoteRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
