using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApi.Data;
using ProjectManagerApi.Models;

namespace ProjectManagerApi.Controllers
{
    [ApiController]
    [Route("projects/{projectId}/schedule")]
    public class SchedulerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SchedulerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSchedule(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId && !t.IsCompleted)
                .ToListAsync();

            if (!tasks.Any())
                return BadRequest(new { message = "No tasks found for scheduling." });

            int GetPriorityWeight(string priority) => priority.ToLower() switch
            {
                "high" => 3,
                "medium" => 2,
                "low" => 1,
                _ => 0
            };

            var orderedTasks = tasks
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)       
                .ThenByDescending(t => GetPriorityWeight(t.Priority))
                .ThenBy(t => t.EstimatedHours ?? double.MaxValue)
                .Select(t => t.Title)
                .ToList();

            return Ok(new { recommendedOrder = orderedTasks });
        }
    }
}
