using MediatR;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.GetAllCommentsByProjectTaskId
{
    public class GetAllCommentsByProjectTaskIdQueryHandler : IRequestHandler<GetAllCommentsByProjectTaskIdQuery, ICollection<CommentDto>>
    {
        private readonly ICommentRepository _commentRepository;
        public GetAllCommentsByProjectTaskIdQueryHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<ICollection<CommentDto>> Handle(GetAllCommentsByProjectTaskIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetAllByProjectTaskIdAsync(request.projectTaskId);

            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Edited = c.Edited,
                AuthorName = c.Author?.UserName
            }).ToList();
        }
    }
}
