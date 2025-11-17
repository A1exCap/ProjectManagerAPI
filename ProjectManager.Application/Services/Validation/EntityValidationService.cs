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
    public class EntityValidationService : IEntityValidationService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ICommentRepository _commentRepository; 
        private readonly ILogger<EntityValidationService> _logger;

        public EntityValidationService(IProjectRepository projectRepository, IProjectTaskRepository projectTaskRepository, 
            ILogger<EntityValidationService> logger, ICommentRepository commentRepository)
        {
            _logger = logger;
            _commentRepository = commentRepository;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
        }

        public async Task<bool> EnsureCommentBelongsToTaskAsync(int commentId, int taskId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} does not exist", commentId);
                throw new NotFoundException($"Comment with ID {commentId} does not exist.");
            }

            if (comment.TaskId != taskId)
            {
                _logger.LogWarning("Comment {CommentId} does not belong to task {TaskId}", commentId, taskId);
                throw new ForbiddenException("Comment does not belong to this task.");
            }

            return true;
        }

        public async Task EnsureProjectExistsAsync(int projectId)
        {
            if (!await _projectRepository.ExistsAsync(projectId))
            {
                _logger.LogWarning("Project {ProjectId} does not exist", projectId);
                throw new NotFoundException($"Project with ID {projectId} does not exist.");
            }
        }

        public async Task<bool> EnsureTaskBelongsToProjectAsync(int taskId, int projectId)
        {
            var task = await _projectTaskRepository.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exist", taskId);
                throw new NotFoundException($"Task with ID {taskId} does not exist.");
            }

            if (task.ProjectId != projectId)
            {
                _logger.LogWarning("Task {TaskId} does not belong to project {ProjectId}", taskId, projectId);
                throw new ForbiddenException("Task does not belong to this project.");
            }

            return true;
        }
    }
}
