using ProjectManagerApi.DTOs;

public interface ISchedulerService
{
    ScheduleResultDto GenerateSchedule(List<TaskDto> tasks);
}


public class SchedulerService : ISchedulerService
{
    public ScheduleResultDto GenerateSchedule(List<TaskDto> tasks)
    {
        var ordered = tasks
            .Where(t => !t.IsCompleted)
.OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .Select(t => t.Title)
            .ToList();

        return new ScheduleResultDto
        {
            RecommendedOrder = ordered
        };
    }
}

