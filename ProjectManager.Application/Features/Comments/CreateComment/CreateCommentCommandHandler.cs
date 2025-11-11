using MediatR;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _uow;
        public CreateCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork uow) 
        {
            _commentRepository = commentRepository;
            _uow = uow;
        }
        public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment { Content = request.Content, ProjectTaskId = request.ProjectTaskId, AuthorId = request.AuthorId };
            await _commentRepository.AddCommentAsync(comment);
            await _uow.SaveChangesAsync(cancellationToken);
            return comment.Id;
        }
    }
}
