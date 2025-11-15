using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
    {
        private readonly ILogger<CreateTaskCommandHandler> _logger;
        private readonly IProjectAccessService _accessService;
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public CreateTaskCommandHandler(IProjectTaskRepository taskRepository, IUnitOfWork unitOfWork, UserManager<User> userManager, 
            IProjectAccessService accessService, ILogger<CreateTaskCommandHandler> logger, IProjectRepository projectRepository)
        {
            _logger = logger;
            _accessService = accessService;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _projectRepository = projectRepository;
        }

        public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateTaskCommandHandler for projectId: {ProjectId}", request.ProjectId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            User? assignee = null;
            if (!string.IsNullOrEmpty(request.dto.AssigneeEmail))
            {
                assignee = await _userManager.FindByEmailAsync(request.dto.AssigneeEmail);
                if (assignee == null)
                {
                    _logger.LogWarning("User with email {AssigneeEmail} does not exist", request.dto.AssigneeEmail);
                    throw new NotFoundException($"User with email '{request.dto.AssigneeEmail}' not found.");
                }
            }

            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, "Manager");

            var task = new ProjectTask
            {
                Title = request.dto.Title,
                Description = request.dto.Description,
                Priority = request.dto.Priority,
                DueDate = request.dto.DueDate,
                EstimatedHours = request.dto.EstimatedHours,
                Tags = request.dto.Tags,
                ProjectId = request.ProjectId,
                AssigneeId = assignee?.Id
            };

            await _taskRepository.AddTaskAsync(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created task with ID {TaskId} for projectId: {ProjectId}", task.Id, request.ProjectId);
            return task.Id;
        }
    }
}
