using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskCompleted;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted
{
    public class MarkTaskStartedCommandHandler : IRequestHandler<MarkTaskStartedCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<MarkTaskStartedCommandHandler> _logger;
        private readonly IProjectAccessService _accessService;
        public MarkTaskStartedCommandHandler(IUnitOfWork unitOfWork, IProjectRepository projectRepository,
            IProjectTaskRepository projectTaskRepository, ILogger<MarkTaskStartedCommandHandler> logger, IProjectAccessService accessService)
        {
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
            _accessService = accessService;
        }
        public async Task<Unit> Handle(MarkTaskStartedCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling MarkTaskStartedCommand for taskId: {TaskId}", request.TaskId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, "Contributor");

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exists", request.TaskId);
                throw new NotFoundException($"Task with ID {request.TaskId} does not exist.");
            }

            task.Status = ProjectTaskStatus.InProgress;

            await _projectTaskRepository.UpdateTaskAsync(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} marked as started successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
