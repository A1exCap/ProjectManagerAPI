using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Domain.Entities;
using ProjectManager_API.Common;
using ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/projects/{projectId}/tasks/{taskId}/comments")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CommentDto>>>> GetAllComments(int projectId, int taskId, [FromQuery] CommentQueryParams queryParams)
        {
            _logger.LogInformation("Getting all comments: projectId={ProjectId}, taskId: {TaskId}", projectId, taskId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetAllCommentsByTaskIdQuery(projectId, userId, taskId, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} comments by taskId: {TaskId}", result.Items.Count(), taskId);
            return Ok(ApiResponseFactory.Success(result, "Comments retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateComment(int projectId, int taskId, [FromBody] CreateCommentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var commentId = await _mediator.Send(new CreateCommentCommand(projectId, taskId, userId, dto.Content));
            return Ok(ApiResponseFactory.Created(commentId));
        }

        [HttpPatch("{commentId}")]
        public async Task<ActionResult<ApiResponse>> UpdateComment(int projectId, int taskId, int commentId, [FromBody] UpdateCommentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new UpdateCommentCommand(projectId, taskId, commentId, userId, dto.Content));
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpDelete("{commentId}")]
        public async Task<ActionResult<ApiResponse>> DeleteComment(int projectId, int taskId, int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new DeleteCommentCommand(projectId, taskId, commentId, userId));
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
