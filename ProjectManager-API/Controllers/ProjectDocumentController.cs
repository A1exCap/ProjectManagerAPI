using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.Features.ProjectDocuments.Commands.DeleteDocumentCommand;
using ProjectManager.Application.Features.ProjectDocuments.Commands.UploadDocumentCommand;
using ProjectManager.Application.Features.ProjectDocuments.Queries.DownloadDocumentByIdQuery;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Application.Features.TaskAttachments.Commands.DeleteAttachmentCommand;
using ProjectManager.Application.Features.TaskAttachments.Queries.DownloadAttachmentByIdQuery;
using ProjectManager_API.Common;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/documents")]
    [Authorize]
    [ApiController]
    public class ProjectDocumentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProjectDocumentController> _logger;

        public ProjectDocumentController(IMediator mediator, ILogger<ProjectDocumentController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProjectDocumentDto>>>> GetDocuments(int projectId, [FromQuery] DocumentQueryParams queryParams)
        {
            _logger.LogInformation("Getting all documents: projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetAllDocumentsByProjectIdQuery(projectId, userId, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} documents by projectId: {TaskId}", result.Items.Count(), projectId);
            return Ok(ApiResponseFactory.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> UploadDocument(int projectId, IFormFile file)
        {
            _logger.LogInformation("Uploading Document: projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var documentId = await _mediator.Send(new UploadDocumentCommand(file, userId, projectId));

            _logger.LogInformation("Request completed: Document uploaded successfully, Document id: {DocumentId}", documentId);
            return Ok(ApiResponseFactory.Created(documentId));
        }

        [HttpGet("{documentId}/download")]
        public async Task<IActionResult> DownloadDocument(int projectId, int documentId)
        {
            _logger.LogInformation("Downloading Document: {DocumentId}", documentId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var document = await _mediator.Send(new DownloadDocumentByIdQuery(projectId, documentId, userId));

            if (document == null)
                return NotFound();

            _logger.LogInformation("Request completed: Document downloaded successfully, Document id: {DocumentId}", documentId);
            return File(document.FileContent, document.ContentType, document.FileName);
        }

        [HttpDelete("{documentId}")]
        public async Task<ActionResult<ApiResponse>> DeleteDocument(int projectId, int documentId)
        {
            _logger.LogInformation("Deleting Document: {DocumentId}", documentId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new DeleteDocumentCommand(projectId, userId, documentId));

            _logger.LogInformation("Request completed: Document deleted successfully, Document id: {DocumentId} ", documentId);
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}

