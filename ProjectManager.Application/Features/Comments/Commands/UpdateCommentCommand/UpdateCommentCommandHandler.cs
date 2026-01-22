using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Commands.UpdateCommentCommand
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Unit>
    {
        private readonly ILogger<UpdateCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        public UpdateCommentCommandHandler(ILogger<UpdateCommentCommandHandler> logger, IEntityValidationService entityValidationService,
            ICommentRepository commentRepository, IUnitOfWork unitOfWork, IAccessService accessService)
        {
            _accessService = accessService;
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _entityValidationService = entityValidationService;
            _logger = logger;
        }
        public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateCommentCommand by commentId: {Comment}", request.CommentId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _entityValidationService.EnsureCommentBelongsToTaskAsync(request.CommentId, request.TaskId);
            await _accessService.EnsureUserIsCommentAuthorAsync(request.UserId, request.CommentId);

            var comment = await _commentRepository.GetByIdAsync(request.CommentId);
            comment.Content = request.dto.Content;
            comment.Edited = true;

            _commentRepository.UpdateComment(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment with ID {CommentId} updated successfully", request.CommentId);
            return Unit.Value;
        }
    }
}
