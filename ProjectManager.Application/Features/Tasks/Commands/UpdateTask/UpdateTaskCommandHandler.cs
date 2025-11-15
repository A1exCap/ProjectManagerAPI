using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;
        private readonly ITaskValidationService _taskValidationService;
        public UpdateTaskCommandHandler(ILogger<UpdateTaskCommandHandler> logger, UserManager<User> userManager,
            IProjectTaskRepository projectTaskRepository, IUnitOfWork unitOfWork, ITaskValidationService taskValidationService)
        {
            _taskValidationService = taskValidationService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateTaskCommand for taskId: {TaskId}", request.TaskId);

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

            var task = await _taskValidationService.ValidateTaskInProjectAsync(request.ProjectId, request.TaskId, request.UserId, "Manager", cancellationToken);

            task.Title = request.dto.Title;
            task.Description = request.dto.Description;
            task.Priority = request.dto.Priority;   
            task.Status = request.dto.Status;
            task.DueDate = request.dto.DueDate;
            task.EstimatedHours = request.dto.EstimatedHours;
            task.Tags = request.dto.Tags;
            task.AssigneeId = assignee?.Id;

            await _projectTaskRepository.UpdateTaskAsync(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} updated successfully", request.TaskId);

            return Unit.Value;
        }
    }
}
