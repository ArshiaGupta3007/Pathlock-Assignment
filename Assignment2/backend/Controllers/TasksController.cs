using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProjectManagerApi.DTOs;
using ProjectManagerApi.Services;

namespace ProjectManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/projects/{projectId:int}/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _tasks;

        private int CurrentUserId =>
            int.TryParse(User.FindFirstValue("id"), out var id) ? id : 0;

        public TasksController(ITaskService tasks)
        {
            _tasks = tasks;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(int projectId)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            var result = await _tasks.GetTasksByProject(CurrentUserId, projectId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(int projectId, [FromBody] TaskCreateDto dto)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _tasks.AddTaskToProject(CurrentUserId, projectId, dto);
            if (created == null)
                return NotFound(new { message = "Project not found or not accessible." });

            return CreatedAtAction(nameof(GetTasks), new { projectId }, created);
        }

        [HttpPut("{taskId:int}")]
        public async Task<IActionResult> UpdateTask(int projectId, int taskId, [FromBody] TaskUpdateDto dto)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _tasks.UpdateTask(CurrentUserId, taskId, dto);
            if (updated == null)
                return NotFound(new { message = "Task not found." });

            return Ok(updated);
        }

        [HttpDelete("{taskId:int}")]
        public async Task<IActionResult> DeleteTask(int projectId, int taskId)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            var deleted = await _tasks.DeleteTask(CurrentUserId, taskId);
            if (!deleted)
                return NotFound(new { message = "Task not found." });

            return NoContent();
        }
    }
}
