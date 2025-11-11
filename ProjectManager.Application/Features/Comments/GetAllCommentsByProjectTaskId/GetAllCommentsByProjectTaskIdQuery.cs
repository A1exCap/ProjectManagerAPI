using MediatR;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.GetAllCommentsByProjectTaskId
{
    public record class GetAllCommentsByProjectTaskIdQuery(int projectTaskId) : IRequest<ICollection<CommentDto>>;
}
