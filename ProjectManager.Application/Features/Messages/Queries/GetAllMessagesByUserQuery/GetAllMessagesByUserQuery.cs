using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Message;
using ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Messages.Queries.GetAllMessagesByUserQuery
{
    public record GetAllMessagesByUserQuery(string UserId, MessageQueryParams QueryParams) : IRequest<PagedResult<MessageDto>>;
}
