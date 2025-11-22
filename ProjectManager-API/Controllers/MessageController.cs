using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Message;
using ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand;
using ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery;
using ProjectManager.Application.Features.Messages.Commands.DeleteMessageCommand;
using ProjectManager.Application.Features.Messages.Commands.DeleteMessagesByUserIdCommand;
using ProjectManager.Application.Features.Messages.Queries.GetAllMessagesByUserQuery;
using ProjectManager_API.Common;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/messages")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMediator mediator, ILogger<MessageController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<MessageDto>>>> GetMessages([FromQuery] MessageQueryParams queryParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Getting all messages by userId: {UserId}", userId);

            var result = await _mediator.Send(new GetAllMessagesByUserQuery(userId, queryParams));

            _logger.LogInformation("Request completed: Retrieved {Count} messages by userId: {UserId}", result.Items.Count(), userId);
            return Ok(ApiResponseFactory.Success(result, "Messages retrieved successfully"));
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse>> DeleteAllMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Deleting all messages by userId: {UserId}", userId);

            await _mediator.Send(new DeleteMessagesByUserIdCommand(userId));

            _logger.LogInformation("Request completed: All messages deleted by userId: {UserId}", userId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpDelete("{messageId}")]
        public async Task<ActionResult<ApiResponse>> DeleteMessage(int messageId)
        {
            _logger.LogInformation("Deleting message by messageId: {MessageId}", messageId);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mediator.Send(new DeleteMessageCommand(userId, messageId));

            _logger.LogInformation("Request completed: Message deleted by messageId: {MessageId}", messageId);
            return Ok(ApiResponseFactory.NoContent());
        }

    }
}
