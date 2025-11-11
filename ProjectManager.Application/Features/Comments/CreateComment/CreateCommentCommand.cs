using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.CreateComment
{
    public record CreateCommentCommand(string Content, int ProjectTaskId) : IRequest<int>
    {
        public string AuthorId { get; init; } = default!;
    };
}
