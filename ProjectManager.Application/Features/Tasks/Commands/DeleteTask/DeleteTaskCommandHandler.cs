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
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, IEntityValidationService entityValidationService,
            IProjectTaskRepository projectTaskRepository, ILogger<DeleteTaskCommandHandler> logger, IAccessService accessService)
        {
            _entityValidationService = entityValidationService;
            _unitOfWork = unitOfWork;
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
            _accessService = accessService;
        }
        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteTaskCommand by taskId: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Owner", "Manager"]);

            await _projectTaskRepository.DeleteTaskByIdAsync(request.TaskId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with id:{TaskId} deleted Succesfully", request.TaskId);

            return Unit.Value;
        }
    }
}
