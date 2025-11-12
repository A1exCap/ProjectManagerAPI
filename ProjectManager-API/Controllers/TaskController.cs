using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Features;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
using ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery;
using ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery;
using ProjectManager.Domain.Entities;
using ProjectManager_API.Common;
using ProjectManager_API.Exceptions;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/tasks")]
    [Authorize]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;
        private readonly ILogger<TaskController> _logger;

        public TaskController(IMediator mediator, UserManager<User> userManager, ILogger<TaskController> logger)
        {
            _userManager = userManager;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ICollection<ProjectTaskDto>>>> GetAllTasksByProjectId(int projectId)
        {
            _logger.LogInformation("Getting all tasks for projectId: {ProjectId}", projectId);

            var tasks = await _mediator.Send(new GetAllTasksByProjectIdQuery(projectId));

            if (tasks == null || !tasks.Any())
                throw new NotFoundException($"No tasks found for projectId {projectId}");

            return Ok(ApiResponseFactory.Success(tasks, "Tasks retrieved successfully"));
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<ApiResponse<ProjectTaskDetailsDto>>> GetTaskById(int taskId)
        {
            _logger.LogInformation("Getting task details for taskId: {TaskId}", taskId);

            var task = await _mediator.Send(new GetTaskByIdQuery(taskId));

            if (task == null)
                throw new NotFoundException($"Task with id {taskId} not found");

            return Ok(ApiResponseFactory.Success(task, "Task retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateTask(int projectId, [FromBody] CreateProjectTaskDto dto)
        {
            _logger.LogInformation("Creating task '{Title}' for projectId: {ProjectId}", dto.Title, projectId);

            var command = new CreateTaskCommand(
                projectId,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.DueDate,
                dto.EstimatedHours,
                dto.ActualHours,
                dto.Tags,
                dto.AssigneeEmail
            );

            var taskId = await _mediator.Send(command);

            return Ok(ApiResponseFactory.Created(taskId, "Task created successfully"));
        }
    }
}
