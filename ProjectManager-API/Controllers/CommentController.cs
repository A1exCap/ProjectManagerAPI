using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Comments.CreateComment;
using ProjectManager.Application.Features.Comments.GetAllCommentsByProjectTaskId;
using ProjectManager.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectManager_API.Controllers
{
    [Route("api/comments")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateComment(CreateCommentCommand command)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null) return Unauthorized();

            var commentId = await _mediator.Send(command with { AuthorId = userId });

            return Ok(commentId);
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Comment>>> GetAllCommentsByProjectTaskId(int id)
        {
            var comments = await _mediator.Send(new GetAllCommentsByProjectTaskIdQuery(id));
            return Ok(comments);
        }
    }
}
