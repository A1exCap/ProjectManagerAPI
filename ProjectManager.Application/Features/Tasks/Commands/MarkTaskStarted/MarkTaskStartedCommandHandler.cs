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
        private readonly ITaskValidationService _taskValidationService;
        public MarkTaskStartedCommandHandler(IUnitOfWork unitOfWork, ITaskValidationService taskValidationService,
            IProjectTaskRepository projectTaskRepository, ILogger<MarkTaskStartedCommandHandler> logger)
        {
            _taskValidationService = taskValidationService;
            _unitOfWork = unitOfWork;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(MarkTaskStartedCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling MarkTaskStartedCommand for taskId: {TaskId}", request.TaskId);

            var task = await _taskValidationService.ValidateTaskInProjectAsync(request.ProjectId, request.TaskId, request.UserId, "Contributor", cancellationToken);

            task.Status = ProjectTaskStatus.InProgress;

            await _projectTaskRepository.UpdateTaskAsync(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} marked as started successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
