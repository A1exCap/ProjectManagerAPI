using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.Access
{
    public interface IAccessService
    {
        Task EnsureUserIsCommentAuthorAsync(string userId, int commentId);
        Task EnsureUserHasAccessAsync(int projectId, string userId);
        Task<bool> EnsureUserHasRoleAsync(int projectId, string userId, string[] allowedRoles);
    }
}
