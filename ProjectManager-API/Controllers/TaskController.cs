using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
using ProjectManager.Application.Features;
using ProjectManager.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery;
using ProjectManager.Application.DTOs.ProjectTask;
using Microsoft.AspNetCore.Identity;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/tasks")]
    [Authorize]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;
        public TaskController(IMediator mediator, UserManager<User> userManager)
        {
            _userManager = userManager; 
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ProjectTaskDto>>> GetAllTasksByProjectId(int projectId)
        {
            var tasks = await _mediator.Send(new GetAllTasksByProjectIdQuery(projectId));
            return Ok(tasks);
        }

        [HttpGet("{taskId}")] 
        public async Task<ActionResult<ICollection<ProjectTaskDetailsDto>>> GetTaskById(int taskId)
        {
            var task = await _mediator.Send(new GetAllTasksByProjectIdQuery(taskId));
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateTask(int projectId, [FromBody] CreateProjectTaskDto dto)
        {
            var command = new CreateTaskCommand(projectId, dto.Title, dto.Description, dto.Priority, dto.DueDate, 
                dto.EstimatedHours, dto.ActualHours, dto.Tags, dto.AssigneeEmail);

            var taskId = await _mediator.Send(command);
            return Ok(taskId);
        }
    }
}
