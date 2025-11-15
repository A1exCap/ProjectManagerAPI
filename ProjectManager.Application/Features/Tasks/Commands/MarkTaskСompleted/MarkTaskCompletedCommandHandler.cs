using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
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
        private readonly ITaskValidationService _taskValidationService;
        public MarkTaskCompletedCommandHandler(IUnitOfWork unitOfWork, ITaskValidationService taskValidationService,
            IProjectTaskRepository projectTaskRepository, ILogger<MarkTaskCompletedCommandHandler> logger) 
        { 
            _taskValidationService = taskValidationService;
            _unitOfWork = unitOfWork;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(MarkTaskCompletedCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling MarkTaskCompletedCommand for taskId: {TaskId}", request.TaskId);

            var task = await _taskValidationService.ValidateTaskInProjectAsync(request.ProjectId, request.TaskId, request.UserId, "Contributor", cancellationToken);

            task.Status = ProjectTaskStatus.Done;
            task.CompletedAt = DateTime.UtcNow;

            await _projectTaskRepository.UpdateTaskAsync(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} marked as completed successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
