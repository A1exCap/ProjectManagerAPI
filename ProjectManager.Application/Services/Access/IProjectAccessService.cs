using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.Access
{
    public interface IProjectAccessService
    {
        Task EnsureUserHasAccessAsync(int projectId, string userId);
        Task EnsureUserHasRoleAsync(int projectId, string userId, string requiredRole);
    }
}
