namespace ProjectManagerApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string Priority { get; set; } = "Medium";
        public double? EstimatedHours { get; set; }
    }
}
