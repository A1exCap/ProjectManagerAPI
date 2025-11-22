using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.MarkTaskCompleted
{
    public class MarkTaskCompletedCommandHandler : IRequestHandler<MarkTaskCompletedCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<MarkTaskCompletedCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICreateMessageService _messageService;
        private readonly IAccessService _accessService;
        public MarkTaskCompletedCommandHandler(IUnitOfWork unitOfWork, IEntityValidationService entityValidationService,
            IProjectTaskRepository projectTaskRepository, ILogger<MarkTaskCompletedCommandHandler> logger, IAccessService accessService,
            ICreateMessageService messageService)
        {
            _entityValidationService = entityValidationService;
            _unitOfWork = unitOfWork;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
            _messageService = messageService;
            _accessService = accessService;
        }
        public async Task<Unit> Handle(MarkTaskCompletedCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling MarkTaskCompletedCommand by taskId: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Contributor"]);

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            task.Status = ProjectTaskStatus.Done;
            task.CompletedAt = DateTime.UtcNow;

            _projectTaskRepository.UpdateTask(task);
            await _messageService.CreateAsync(task.CreatorId, NotificationType.TaskCompleted, $"Task '{task.Title}' has been marked as completed.", RelatedEntityType.ProjectTask, task.Id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} marked as completed successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
