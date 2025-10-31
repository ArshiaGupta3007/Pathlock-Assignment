using System.ComponentModel.DataAnnotations;

namespace ProjectManagerApi.DTOs
{
    public class TaskCreateDto
    {
        [Required]
        public string Title { get; set; } = null!;
        public DateTime? DueDate { get; set; }
    }

    public class TaskUpdateDto
    {
        [Required]
        public string Title { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int ProjectId { get; set; }
    }
}
