using MediatR;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services
{
    public class TaskValidationService : ITaskValidationService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectAccessService _accessService;
        private readonly Logger<TaskValidationService> _logger;

        public TaskValidationService(IProjectRepository projectRepository, IProjectTaskRepository projectTaskRepository, 
            IProjectAccessService accessService, Logger<TaskValidationService> logger)
        {
            _logger = logger;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _accessService = accessService;
        }

        public async Task<ProjectTask> ValidateTaskInProjectAsync(int projectId, int taskId, string userId, string requiredRole, 
            CancellationToken cancellationToken)
        {
            var projectExists = await _projectRepository.ExistsAsync(projectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {projectId} does not exist", projectId);
                throw new NotFoundException($"Project with ID {projectId} does not exist.");
            }

            await _accessService.EnsureUserHasRoleAsync(projectId, userId, requiredRole);

            var task = await _projectTaskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {taskId} does not exists", taskId);
                throw new NotFoundException($"Task with ID {taskId} does not exist.");
            }

            if (task.ProjectId != projectId)
            {
                _logger.LogWarning("Task with ID {taskId} does not  belong to project {pojectId}", taskId, projectId);
                throw new NotFoundException($"Task {taskId} does not belong to project {projectId}.");
            }

            return task;
        }
    }
}
