using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
using ProjectManager.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskCompleted;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted;
using ProjectManager.Application.Features.Tasks.Commands.UpdateTask;
using ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery;
using ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery;
using ProjectManager_API.Common;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/tasks")]
    [Authorize]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskController> _logger;

        public TaskController(IMediator mediator, ILogger<TaskController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ICollection<ProjectTaskDto>>>> GetAllTasksByProjectId(int projectId)
        {
            _logger.LogInformation("Getting all tasks for projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _mediator.Send(new GetAllTasksByProjectIdQuery(projectId, userId));

            _logger.LogInformation("Request completed: Retrieved {Count} tasks for projectId={projectId}", tasks.Count, projectId);
            return Ok(ApiResponseFactory.Success(tasks, "Tasks retrieved successfully"));
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<ApiResponse<ProjectTaskDetailsDto>>> GetTaskById(int taskId)
        {
            _logger.LogInformation("Getting task details for taskId: {TaskId}", taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var task = await _mediator.Send(new GetTaskByIdQuery(taskId, userId));

            _logger.LogInformation("Request completed: Task details retrieved for taskId={taskId}", taskId);
            return Ok(ApiResponseFactory.Success(task, "Task retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateTask(int projectId, [FromBody] CreateProjectTaskDto dto)
        {
            _logger.LogInformation("Creating task '{Title}' for projectId: {ProjectId}", dto.Title, projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new CreateTaskCommand(
                projectId,
                userId,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.DueDate,
                dto.EstimatedHours,
                dto.Tags,
                dto.AssigneeEmail
            );

            var taskId = await _mediator.Send(command);

            _logger.LogInformation("Request completed: Task created succesfully, task id:{taskId}", taskId);
            return Ok(ApiResponseFactory.Created(taskId, "Task created successfully"));
        }

        [HttpPut("{taskId}")]
        public async Task<ActionResult<ApiResponse>> UpdateTaskById(int taskId, int projectId, [FromBody] ProjectTaskUpdateDto dto)
        {
            _logger.LogInformation("Updating task details with taskId: {TaskId}", taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new UpdateTaskCommand(
                projectId,
                taskId, 
                userId,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.Status,
                dto.DueDate,
                dto.EstimatedHours,
                dto.Tags,
                dto.AssigneeEmail
                ));

            _logger.LogInformation("Request updated: Task details updated with taskId:{taskId}", taskId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpPatch("{taskId}/mark-completed")]
        public async Task<ActionResult<ApiResponse>> MarkAsCompletedTaskById(int taskId, int projectId)
        {
            _logger.LogInformation("Marking task as completed with taskId: {TaskId}", taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new MarkTaskCompletedCommand(
                projectId,
                taskId,
                userId
                ));

            _logger.LogInformation("Request completed: Task marked as completed with taskId:{taskId}", taskId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpPatch("{taskId}/mark-started")]
        public async Task<ActionResult<ApiResponse>> MarkAsStartedTaskById(int taskId, int projectId)
        {
            _logger.LogInformation("Marking task as started with taskId: {TaskId}", taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new MarkTaskStartedCommand(
                projectId,
                taskId,
                userId
                ));

            _logger.LogInformation("Request completed: Task marked as started with taskId:{taskId}", taskId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpDelete("{taskId}")]
        public async Task<ActionResult<ApiResponse>> DeleteTaskById(int taskId, int projectId)
        {
            _logger.LogInformation("Deleting task with taskId: {TaskId}", taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new DeleteTaskCommand(
                projectId,
                taskId,
                userId
                ));

            _logger.LogInformation("Request completed: Task deleted with taskId:{taskId}", taskId);
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
