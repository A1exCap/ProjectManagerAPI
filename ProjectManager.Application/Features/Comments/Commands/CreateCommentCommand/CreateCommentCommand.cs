using MediatR;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.ProjectTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Commands.CreateCommentCommand
{
    public record CreateCommentCommand(int ProjectId, int TaskId, string UserId, CreateCommentDto dto) : IRequest<int>;

}
