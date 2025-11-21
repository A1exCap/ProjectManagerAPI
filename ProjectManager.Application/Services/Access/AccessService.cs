using Microsoft.Extensions.Logging;
using ProjectManager.Application.Exceptions;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ProjectManager.Application.Services.Access
{
    public class AccessService : IAccessService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<AccessService> _logger;
        public AccessService(IProjectUserRepository projectUserRepository, ILogger<AccessService> logger, 
            ICommentRepository commentRepository, IProjectRepository projectRepository)
        {
            _logger = logger;
            _commentRepository = commentRepository;
            _projectUserRepository = projectUserRepository;
            _projectRepository = projectRepository;
        }

        public async Task EnsureUserIsCommentAuthorAsync(string userId, int commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} does not exist", commentId);
                throw new NotFoundException($"Comment with ID {commentId} does not exist.");
            }

            if (comment.AuthorId != userId)
            {
                _logger.LogWarning("User {UserId} is not the author of comment {CommentId}", userId, commentId);
                throw new ForbiddenException("You are not the author of this comment.");
            }
        }
        public async Task EnsureUserHasAccessAsync(int projectId, string userId)
        {
            var project = await _projectRepository.GetByProjectIdAsync(projectId);

            if (project.Visibility == ProjectVisibility.Public)
            {
                return;
            }

            var exists = await _projectUserRepository.ExistsAsync(projectId, userId);

            if (!exists)
            {
                _logger.LogWarning("Access denied for user {UserId} to project {ProjectId}", userId, projectId);
                throw new ForbiddenException("You do not have access to this project.");
            }
        }

        public async Task<bool> EnsureUserHasRoleAsync(int projectId, string userId, string[] allowedRoles)
        {
            var role = await _projectUserRepository.GetUserRoleAsync(projectId, userId);

            await EnsureUserHasAccessAsync(projectId, userId);

            if (role == null)
            {
                _logger.LogWarning("Access denied for user {UserId} to project {ProjectId}", userId, projectId);
                throw new ForbiddenException("You do not have access to this project.");
            }

            for(int i = 0; i < allowedRoles.Length; i++)
            {
                if (string.Equals(role, allowedRoles[i], StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            _logger.LogWarning("User {UserId} does not have required role in project {ProjectId}", userId, projectId);
            throw new ForbiddenException($"User does not have permission for this action.");
        }

        public async Task EnsureUserIsProjectOwnerAsync(int projectId, string userId)
        {
            var project = await _projectRepository.GetByProjectIdAsync(projectId);

            if (project.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not the owner of project {ProjectId}", userId, projectId);
                throw new ForbiddenException("User is not the owner of this project.");
            }
        }
    }
}
