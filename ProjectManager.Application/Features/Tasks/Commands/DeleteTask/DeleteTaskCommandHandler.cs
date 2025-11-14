using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;
        private readonly IProjectAccessService _accessService;
        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, IProjectRepository projectRepository,
            IProjectTaskRepository projectTaskRepository, ILogger<DeleteTaskCommandHandler> logger, IProjectAccessService accessService)
        {
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
            _accessService = accessService;
        }
        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteTaskCommand by taskId: {TaskId}", request.TaskId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, "Manager");

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exists", request.TaskId);
                throw new NotFoundException($"Task with ID {request.TaskId} does not exist.");
            }

            await _projectTaskRepository.DeleteTaskByIdAsync(task.Id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with id:{TaskId} deleted Succesfully", request.TaskId);

            return Unit.Value;
        }
    }
}
