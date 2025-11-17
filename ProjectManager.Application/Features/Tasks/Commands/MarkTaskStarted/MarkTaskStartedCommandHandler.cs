using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskCompleted;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted
{
    public class MarkTaskStartedCommandHandler : IRequestHandler<MarkTaskStartedCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<MarkTaskStartedCommandHandler> _logger;
        private readonly IAccessService _accessService;  
        private readonly IEntityValidationService _entityValidationService;
        public MarkTaskStartedCommandHandler(IUnitOfWork unitOfWork, IAccessService accessService,
            IProjectTaskRepository projectTaskRepository, ILogger<MarkTaskStartedCommandHandler> logger, IEntityValidationService entityValidationService)
        {
            _accessService = accessService;
            _entityValidationService = entityValidationService;
            _unitOfWork = unitOfWork;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(MarkTaskStartedCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling MarkTaskStartedCommand by taskId: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Contributor"]);

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            task.Status = ProjectTaskStatus.InProgress;

            _projectTaskRepository.UpdateTask(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} marked as started successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
