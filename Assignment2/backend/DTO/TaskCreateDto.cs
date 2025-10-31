namespace ProjectManagerApi.DTO
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
