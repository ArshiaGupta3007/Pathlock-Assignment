using Microsoft.EntityFrameworkCore;
using ProjectManagerApi.Data;
using ProjectManagerApi.DTOs;
using ProjectManagerApi.Models;

namespace ProjectManagerApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskDto?> AddTaskToProject(int userId, int projectId, TaskCreateDto dto)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
                return null;

            var task = new TaskItem
            {
                Title = dto.Title,
                DueDate = dto.DueDate,
                IsCompleted = false,
                ProjectId = projectId
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                ProjectId = task.ProjectId
            };
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByProject(int userId, int projectId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
                return Enumerable.Empty<TaskDto>();

            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    ProjectId = t.ProjectId
                })
                .ToListAsync();
        }

        public async Task<TaskDto?> UpdateTask(int userId, int taskId, TaskUpdateDto dto)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.OwnerId == userId);

            if (task == null)
                return null;

            task.Title = dto.Title;
            task.DueDate = dto.DueDate;
            task.IsCompleted = dto.IsCompleted;

            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                ProjectId = task.ProjectId
            };
        }

        public async Task<bool> DeleteTask(int userId, int taskId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.OwnerId == userId);

            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleTaskComplete(int userId, int taskId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.OwnerId == userId);

            if (task == null)
                return false;

            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
