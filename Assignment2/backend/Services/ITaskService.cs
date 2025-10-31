using ProjectManagerApi.DTOs;

namespace ProjectManagerApi.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksByProject(int userId, int projectId);
        Task<TaskDto?> AddTaskToProject(int userId, int projectId, TaskCreateDto dto);
        Task<TaskDto?> UpdateTask(int userId, int taskId, TaskUpdateDto dto);
        Task<bool> DeleteTask(int userId, int taskId);
        Task<bool> ToggleTaskComplete(int userId, int taskId);
    }
}
