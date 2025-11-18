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
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services
{
    public class EntityValidationService : IEntityValidationService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        private readonly ILogger<EntityValidationService> _logger;
        private readonly IProjectDocumentRepository _projectDocumentRepository;

        public EntityValidationService(IProjectRepository projectRepository, IProjectTaskRepository projectTaskRepository, 
            ILogger<EntityValidationService> logger, ICommentRepository commentRepository, ITaskAttachmentRepository taskAttachmentRepository,
            IProjectDocumentRepository projectDocumentRepository)
        {
            _logger = logger;
            _commentRepository = commentRepository;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _taskAttachmentRepository = taskAttachmentRepository;
            _projectDocumentRepository = projectDocumentRepository;
        }

        public async Task EnsureDocumentBelongsToProjectAsync(int documentId, int projectId)
        {
            var document = await _projectDocumentRepository.GetDocumentByIdAsync(documentId);

            if (document == null)
            {
                _logger.LogWarning("Document {DocumentId} does not exist", documentId);
                throw new NotFoundException($"Document with ID {documentId} does not exist.");
            }

            if (document.ProjectId != projectId)
            {
                _logger.LogWarning("Document {DocumentId} does not belong to project {ProjectId}", documentId, projectId);
                throw new ForbiddenException("Document does not belong to this project.");
            }
        }

        public async Task EnsureAttachmentBelongsToTaskAsync(int attachmentId, int taskId)
        {
            var attachment = await _taskAttachmentRepository.GetAttachmentByIdAsync(attachmentId);

            if (attachment == null)
            {
                _logger.LogWarning("Attachemnt {AttachmentId} does not exist", attachmentId);
                throw new NotFoundException($"Attachment with ID {attachmentId} does not exist.");
            }

            if (attachment.ProjectTaskId != taskId)
            {
                _logger.LogWarning("Attachemnt {AttachmentId} does not belong to task {TaskId}", attachmentId, taskId);
                throw new ForbiddenException("Attachemnt does not belong to this task.");
            }
        }

        public async Task EnsureCommentBelongsToTaskAsync(int commentId, int taskId)
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
        }
        public async Task EnsureProjectExistsAsync(int projectId)
        {
            if (!await _projectRepository.ExistsAsync(projectId))
            {
                _logger.LogWarning("Project {ProjectId} does not exist", projectId);
                throw new NotFoundException($"Project with ID {projectId} does not exist.");
            }
        }
        public async Task EnsureTaskBelongsToProjectAsync(int taskId, int projectId)
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
        }

        public async Task EnsureUserIsProjectMemberAsync(int projectId, string userId)
        {
            var project = await _projectRepository.GetByProjectIdAsync(projectId);

            if(!project.ProjectUsers.Any(pu => pu.UserId == userId && pu.ProjectId == projectId))
            {
                _logger.LogWarning("User {UserId} is not a member of project {ProjectId}", userId, projectId);
                throw new ForbiddenException("User is not a member of this project.");
            }
        }

        public ProjectUserRole EnsureRoleIsValid(string roleName)
        {
            if (!Enum.TryParse<ProjectUserRole>(roleName, out ProjectUserRole role))
            {
                _logger.LogError("Invalid role provided: {UserRole}", role);
                throw new ValidationException("Invalid role provided.");
            }
            return role;
        }
    }
}
