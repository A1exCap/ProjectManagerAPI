using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery
{
    public record GetAllCommentsByTaskIdQuery(int ProjectId, string UserId, int TaskId, CommentQueryParams QueryParams) : IRequest<PagedResult<CommentDto>>;
}
