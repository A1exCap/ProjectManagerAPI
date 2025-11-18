using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.TaskDocument;
using ProjectManager.Application.Features.TaskAttachments.Commands.DeleteAttachmentCommand;
using ProjectManager.Application.Features.TaskAttachments.Commands.UploadTaskAttachmentCommand;
using ProjectManager.Application.Features.TaskAttachments.Queries.DownloadAttachmentByIdQuery;
using ProjectManager.Application.Features.TaskAttachments.Queries.GetAllAttachmentsByTaskIdQuery;
using ProjectManager_API.Common;
using System.Net.Mail;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/tasks/{taskId}/attachments")]
    [Authorize]
    [ApiController]
    public class TaskAttachmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskAttachmentController> _logger;

        public TaskAttachmentController(IMediator mediator, ILogger<TaskAttachmentController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TaskAttachmentDto>>>> GetAttachments(int projectId, int taskId, 
            [FromQuery] AttachmentQueryParams queryParams)
        {
            _logger.LogInformation("Getting all attachments: projectId: {ProjectId}, taskId: {TaskId}", projectId, taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetAllAttachmentsByTaskIdQuery(projectId, userId, taskId, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} attachments by taskId: {TaskId}", result.Items.Count(), taskId);
            return Ok(ApiResponseFactory.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> UploadAttachment(int projectId, int taskId, IFormFile file)
        {
            _logger.LogInformation("Uploading Attachment: projectId: {ProjectId}, taskId: {TaskId}", projectId, taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var attachmentId = await _mediator.Send(new UploadAttachmentCommand(taskId, file, userId, projectId));

            _logger.LogInformation("Request completed: File uploaded successfully, file id: {AttachmentId}", attachmentId);
            return Ok(ApiResponseFactory.Created(attachmentId));
        }

        [HttpGet("{attachmentId}/download")]
        public async Task<IActionResult> DownloadAttachment(int projectId, int taskId, int attachmentId)
        {
            _logger.LogInformation("Downloading Attachment: {AttachmentId}", attachmentId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var attachment = await _mediator.Send(new DownloadAttachmentByIdQuery(projectId, taskId, attachmentId, userId));

            if (attachment == null)
                return NotFound();

            _logger.LogInformation("Request completed: File downloaded successfully, file id: {AttachmentId}", attachmentId);
            return File(attachment.FileContent, attachment.ContentType, attachment.FileName);
        }

        [HttpDelete("{attachmentId}")]
        public async Task<ActionResult<ApiResponse>> DeleteAttachment(int projectId, int taskId, int attachmentId)
        {
            _logger.LogInformation("Deleting Attachment: {AttachmentId}", attachmentId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new DeleteAttachmentCommand(projectId, taskId, userId, attachmentId));

            _logger.LogInformation("Request completed: File deleted successfully, file id:{AttachmentId} ", attachmentId);
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
