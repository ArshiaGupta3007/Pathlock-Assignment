using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProjectManagerApi.DTOs;
using ProjectManagerApi.Services;

namespace ProjectManagerApi.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projects;
        private readonly ITaskService _tasks;
        private readonly ISchedulerService _scheduler;

        private int CurrentUserId
        {
            get
            {
                var idClaim = User.FindFirstValue("id");
                return int.TryParse(idClaim, out var id) ? id : 0;
            }
        }

        public ProjectsController(IProjectService projects, ITaskService tasks, ISchedulerService scheduler)
        {
            _projects = projects;
            _tasks = tasks;
            _scheduler = scheduler;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            var projects = await _projects.GetUserProjects(CurrentUserId);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDto dto)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _projects.CreateProject(CurrentUserId, dto);
            return CreatedAtAction(nameof(GetProjectById), new { id = created.Id }, created);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            var project = await _projects.GetProject(CurrentUserId, id);
            if (project == null)
                return NotFound(new { message = "Project not found." });

            var tasks = await _tasks.GetTasksByProject(CurrentUserId, id);
            return Ok(new { Project = project, Tasks = tasks });
        }

        [HttpGet("{projectId:int}/project-tasks")]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            var project = await _projects.GetProject(CurrentUserId, projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });

            var tasks = await _tasks.GetTasksByProject(CurrentUserId, projectId);
            return Ok(tasks);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (CurrentUserId == 0)
                return Unauthorized(new { message = "Invalid or missing user token." });

            var deleted = await _projects.DeleteProject(CurrentUserId, id);
            return deleted ? NoContent() : NotFound(new { message = "Project not found." });
        }

        

        

        [HttpPost("{projectId:int}/schedule")]
public async Task<IActionResult> GenerateSchedule(int projectId)
{
    if (CurrentUserId == 0)
        return Unauthorized(new { message = "Invalid or missing user token." });

    var project = await _projects.GetProject(CurrentUserId, projectId);
    if (project == null)
        return NotFound(new { message = "Project not found." });

    var tasks = await _tasks.GetTasksByProject(CurrentUserId, projectId);
    if (tasks == null || !tasks.Any())
        return BadRequest(new { message = "No tasks found for this project." });

var schedule = _scheduler.GenerateSchedule(tasks.ToList());
    return Ok(schedule);
}




    }
}
