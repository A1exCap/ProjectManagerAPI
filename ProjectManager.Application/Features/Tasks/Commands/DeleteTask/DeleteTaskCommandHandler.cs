using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;
        private readonly ITaskValidationService _taskValidationService;
        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, ITaskValidationService taskValidationService,
            IProjectTaskRepository projectTaskRepository, ILogger<DeleteTaskCommandHandler> logger)
        {
            _taskValidationService = taskValidationService;
            _unitOfWork = unitOfWork;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteTaskCommand by taskId: {TaskId}", request.TaskId);

            var task = await _taskValidationService.ValidateTaskInProjectAsync(request.ProjectId, request.TaskId, request.UserId, "Manager", cancellationToken);

            await _projectTaskRepository.DeleteTaskByIdAsync(task.Id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with id:{TaskId} deleted Succesfully", request.TaskId);

            return Unit.Value;
        }
    }
}
