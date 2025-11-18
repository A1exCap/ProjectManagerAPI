using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.ProjectUser;
using ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand;
using ProjectManager.Application.Features.Comments.Commands.UpdateCommentCommand;
using ProjectManager.Application.Features.ProjectDocuments.Commands.UploadDocumentCommand;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Application.Features.ProjectUsers.Commands.CreateProjectUserCommand;
using ProjectManager.Application.Features.ProjectUsers.Commands.DeleteUserFromProjectCommand;
using ProjectManager.Application.Features.ProjectUsers.Commands.UpdateUserRoleCommand;
using ProjectManager.Application.Features.ProjectUsers.Queries.GetAllUsersByProjectIdQuery;
using ProjectManager_API.Common;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/users")]
    [Authorize]
    [ApiController]
    public class ProjectUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProjectUserController> _logger;

        public ProjectUserController(IMediator mediator, ILogger<ProjectUserController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProjectUserDto>>>> GetUsers(int projectId, [FromQuery] UsersQueryParams queryParams)
        {
            _logger.LogInformation("Getting all users: projectId: {ProjectId}", projectId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetAllUsersByProjectIdQuery(projectId, userId, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} users by projectId: {ProjectId}", result.Items.Count(), projectId);
            return Ok(ApiResponseFactory.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> AddUserToProject(int projectId, [FromBody] AddUserToProjectDto dto)
        {
            _logger.LogInformation("Adding user to project: projectId: {ProjectId}, userId: {UserId}", projectId, dto.UserToAddId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var projectUserId = await _mediator.Send(new CreateProjectUserCommand(projectId, userId, dto));

            _logger.LogInformation("Request completed: User added to project successfully, user id: {UserId}, project id: {ProjectId}", dto.UserToAddId, projectId);
            return Ok(ApiResponseFactory.Created(projectUserId));
        }

        [HttpPatch("{userId}")]
        public async Task<ActionResult<ApiResponse>> UpdateUserRoleInProject(int projectId, string userId, string newRole)
        {
            _logger.LogInformation("Updating user role of user with id: {UserId} in project with id {ProjectId}", userId, projectId);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _mediator.Send(new UpdateUserRoleCommand(projectId, currentUserId, userId, newRole));

            _logger.LogInformation("Request updated: User role updated in project with id: {ProjectId}, updated user id: {UserToUpdateId}", projectId, userId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<ApiResponse>> DeleteUserFromProject(int projectId, string userId)
        {
            _logger.LogInformation("Deleting user from project. userId: {UserId}, projectId: {ProjectId}", userId, projectId);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new DeleteUserFromProjectCommand(projectId, userId, currentUserId));

            _logger.LogInformation("Request completed: User deleted seccessfully, userId: {UserId}, projectId: {ProjectId}", userId, projectId);
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
