using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ICommentRepository _commentRepository; 
        public DeleteCommentCommandHandler(IUnitOfWork unitOfWork, IEntityValidationService entityValidationService, ICommentRepository commentRepository,
            ILogger<DeleteCommentCommandHandler> logger, IAccessService accessService)
        {
            _entityValidationService = entityValidationService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _accessService = accessService;
            _commentRepository = commentRepository;
        }
        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteCommentCommand by commentId: {CommentId}", request.CommentId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _entityValidationService.EnsureCommentBelongsToTaskAsync(request.CommentId, request.TaskId);
            await _accessService.EnsureUserIsCommentAuthorAsync(request.UserId, request.CommentId);
          
            await _commentRepository.DeleteCommentByIdAsync(request.CommentId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with id:{TaskId} deleted Succesfully", request.TaskId);

            return Unit.Value;
        }
    }
}
