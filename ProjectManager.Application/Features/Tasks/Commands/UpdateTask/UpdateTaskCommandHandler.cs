using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;
        private readonly IProjectAccessService _accessService;
        public UpdateTaskCommandHandler(ILogger<UpdateTaskCommandHandler> logger, IProjectRepository projectRepository, UserManager<User> userManager,
            IProjectAccessService accessService, IProjectTaskRepository projectTaskRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _accessService = accessService;
            _logger = logger;
            _projectRepository = projectRepository;
            _userManager = userManager;
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateTaskCommand for taskId: {TaskId}", request.TaskId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            User? assignee = null;
            if (!string.IsNullOrEmpty(request.AssigneeEmail))
            {
                assignee = await _userManager.FindByEmailAsync(request.AssigneeEmail);
                if (assignee == null)
                {
                    _logger.LogWarning("User with email {AssigneeEmail} does not exist", request.AssigneeEmail);
                    throw new NotFoundException($"User with email '{request.AssigneeEmail}' not found.");
                }
            }

            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, "Manager");

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exists", request.TaskId);
                throw new NotFoundException($"Task with ID {request.TaskId} does not exist.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;   
            task.Status = request.Status;
            task.DueDate = request.DueDate;
            task.EstimatedHours = request.EstimatedHours;
            task.Tags = request.Tags;
            task.AssigneeId = assignee?.Id;

            await _projectTaskRepository.UpdateTaskAsync(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} updated successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
