using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Application.Features.Projects.Commands.CreateProjectCommand;
using ProjectManager.Application.Features.Projects.Commands.DeleteProjectCommand;
using ProjectManager.Application.Features.Projects.Commands.UpdateProjectCommand;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using ProjectManager.Application.Features.Projects.Queries.GetAllPublicProjectsByNameQuery;
using ProjectManager.Application.Features.Projects.Queries.GetProjectDetailsByIdQuery;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
using ProjectManager.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManager.Application.Features.Tasks.Commands.UpdateTask;
using ProjectManager_API.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects")]
    [Authorize]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IMediator _mediator;
        public ProjectController(ILogger<ProjectController> logger, IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("involved")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProjectDto>>>> GetUserProjects([FromQuery] ProjectQueryParams queryParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Getting all projects: userId: {UserId}", userId);
            var result = await _mediator.Send(new GetAllProjectsByUserIdQuery(userId, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} projects by userId: {UserId}", result.Items.Count(), userId);
            return Ok(ApiResponseFactory.Success(result, "Projects retrieved successfully"));
        }

        [HttpGet("public")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProjectDto>>>> GetPublicProjects(string projectName, [FromQuery] ProjectQueryParams queryParams)
        {
            _logger.LogInformation("Getting all public projects by name: {ProjectName}", projectName);

            var result = await _mediator.Send(new GetAllPublicProjectsByNameQuery(projectName, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} projects by name: {ProjectId}", result.Items.Count(), projectName);
            return Ok(ApiResponseFactory.Success(result, "Projects retrieved successfully"));
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ApiResponse<ProjectDetailsDto>>> GetProjectDetails(int projectId)
        {
            _logger.LogInformation("Getting project details by projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetProjectDetailsByIdQuery(projectId, userId));

            _logger.LogInformation("Request completed: Project details retrieved by projectId: {ProjectId}", projectId);
            return Ok(ApiResponseFactory.Success(result, "Project details retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateProject([FromBody] CreateProjectDto dto)
        {
            _logger.LogInformation("Creating project with name {ProjectName}", dto.Name);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new CreateProjectCommand(userId, dto);

            var projectId = await _mediator.Send(command);

            _logger.LogInformation("Request completed: Project created succesfully, project id: {ProjectId}", projectId);
            return Ok(ApiResponseFactory.Created(projectId, "Project created successfully"));
        }

        [HttpPut("{projectId}")]
        public async Task<ActionResult<ApiResponse>> UpdateProject(int projectId, [FromBody] ProjectUpdateDto dto)
        {
            _logger.LogInformation("Updating project details by projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new UpdateProjectCommand(projectId, userId, dto));

            _logger.LogInformation("Request updated: Project details updated by projectId: {ProjectId}", projectId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpDelete("{projectId}")]
        public async Task<ActionResult<ApiResponse>> DeleteProject(int projectId)
        {
            _logger.LogInformation("Deleting Project by projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new DeleteProjectCommand(projectId, userId));

            _logger.LogInformation("Request completed: Project deleted by projectId: {ProjectId}", projectId);
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
