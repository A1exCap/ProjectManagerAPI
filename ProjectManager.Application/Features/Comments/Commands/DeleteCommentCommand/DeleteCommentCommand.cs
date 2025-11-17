using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand
{
    public record DeleteCommentCommand(int ProjectId, int TaskId, string UserId, int CommentId) : IRequest<Unit>;
}
