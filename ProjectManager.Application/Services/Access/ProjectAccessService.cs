using Microsoft.Extensions.Logging;
using ProjectManager.Application.Exceptions;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.Access
{
    public class ProjectAccessService : IProjectAccessService
    {
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly ILogger<ProjectAccessService> _logger;
        public ProjectAccessService(IProjectUserRepository projectUserRepository, ILogger<ProjectAccessService> logger)
        {
            _logger = logger;
            _projectUserRepository = projectUserRepository;
        }

        public async Task EnsureUserHasAccessAsync(int projectId, string userId)
        {
            var exists = await _projectUserRepository.ExistsAsync(projectId, userId);

            if (!exists)
            {
                _logger.LogWarning("Access denied for user {UserId} to project {ProjectId}", userId, projectId);
                throw new ForbiddenException("You do not have access to this project.");
            }
        }

        public async Task EnsureUserHasRoleAsync(int projectId, string userId, string requiredRole)
        {
            var role = await _projectUserRepository.GetUserRoleAsync(projectId, userId);

            if (role == null)
            {
                _logger.LogWarning("Access denied for user {UserId} to project {ProjectId}", userId, projectId);
                throw new ForbiddenException("You do not have access to this project.");
            }

            if (!string.Equals(role, requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("User {UserId} does not have required role {RequiredRole} in project {ProjectId}", userId, requiredRole, projectId);
                throw new ForbiddenException($"Requires role '{requiredRole}'.");
            }
        }
    }
}
