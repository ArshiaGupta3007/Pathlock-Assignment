using Microsoft.EntityFrameworkCore;
using ProjectManagerApi.Data;
using ProjectManagerApi.DTOs;
using ProjectManagerApi.Models;

namespace ProjectManagerApi.Services
{
    public interface IProjectService
    {
        Task<ProjectDto[]> GetUserProjects(int userId);
        Task<ProjectDto> CreateProject(int userId, ProjectCreateDto dto);
        Task<bool> DeleteProject(int userId, int projectId);
        Task<ProjectDto?> GetProject(int userId, int projectId);
        Task<ProjectDto?> UpdateProjectTitle(int userId, int projectId, string newTitle);
    }

    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _db;

        public ProjectService(AppDbContext db)
        {
            _db = db;
        }

        // 🔹 Get all projects of the current user
        public async Task<ProjectDto[]> GetUserProjects(int userId)
        {
            return await _db.Projects
                .Where(p => p.OwnerId == userId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt
                })
                .ToArrayAsync();
        }

        // 🔹 Update project title
        public async Task<ProjectDto?> UpdateProjectTitle(int userId, int projectId, string newTitle)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null) return null;

            project.Title = newTitle;
            await _db.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }

        // 🔹 Create a new project
        public async Task<ProjectDto> CreateProject(int userId, ProjectCreateDto dto)
        {
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }

        // 🔹 Delete project
        public async Task<bool> DeleteProject(int userId, int projectId)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
                return false;

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return true;
        }

        // 🔹 Get a single project
        public async Task<ProjectDto?> GetProject(int userId, int projectId)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
                return null;

            return new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }
    }
}
