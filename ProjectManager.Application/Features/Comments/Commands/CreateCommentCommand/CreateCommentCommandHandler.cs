using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Commands.CreateCommentCommand
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
    {
        private readonly ILogger<CreateCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICreateMessageService _messageService;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        private readonly IProjectTaskRepository _projectTaskRepository;
        public CreateCommentCommandHandler(ILogger<CreateCommentCommandHandler> logger, IEntityValidationService entityValidationService, 
            ICommentRepository commentRepository, IUnitOfWork unitOfWork, IAccessService accessService, 
            ICreateMessageService messageService, IProjectTaskRepository projectTaskRepository)
        {
            _messageService = messageService;
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _entityValidationService = entityValidationService;
            _logger = logger;
            _accessService = accessService;
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateCommentCommand with CommentText: {Content}", request.dto.Content);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var comment = new Comment
            {
                Content = request.dto.Content,
                TaskId = request.TaskId,
                AuthorId = request.UserId
            };

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            await _commentRepository.AddCommentAsync(comment);
            await _messageService.CreateAsync(task.AssigneeId, NotificationType.NewComment, $"New comment on task: {task.Title}", RelatedEntityType.Comment, task.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created comment with ID {CommentId} for TaskId: {TaskId}", comment.Id, request.TaskId);
            return comment.Id;
        }
    }
}
