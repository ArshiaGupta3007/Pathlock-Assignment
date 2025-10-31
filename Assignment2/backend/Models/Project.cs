namespace ProjectManagerApi.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public List<TaskItem> Tasks { get; set; } = new();
    }
}
