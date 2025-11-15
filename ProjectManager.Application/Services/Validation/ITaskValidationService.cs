using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.Validation
{
    public interface ITaskValidationService
    {
        Task<ProjectTask> ValidateTaskInProjectAsync(int projectId, int taskId, string userId, string requiredRole,
            CancellationToken cancellationToken);
    }
}
