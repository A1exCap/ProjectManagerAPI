using MediatR;
using ProjectManager.Application.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Commands.UpdateCommentCommand
{
    public record UpdateCommentCommand(int ProjectId, int TaskId, int CommentId, string UserId, UpdateCommentDto dto) : IRequest<Unit>;
}
