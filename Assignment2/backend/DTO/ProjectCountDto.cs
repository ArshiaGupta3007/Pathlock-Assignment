namespace ProjectManagerApi.DTOs
{
    public class ProjectCountDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
    }
}
