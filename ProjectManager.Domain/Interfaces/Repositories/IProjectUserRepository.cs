using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectUserRepository
    {
        void DeleteProjectUser(ProjectUser projectUser);
        Task AddProjectUserAsync(ProjectUser projectUser);
        IQueryable<ProjectUser> GetAllUsersByProjectId(int projectId);
        void UpdateProjectUser(ProjectUser projectUser);
        Task<ProjectUser> GetProjectUserIdAsync(int projectId, string userId);
        Task<string> GetUserRoleAsync(int projectId, string userId);
        Task<bool> ExistsAsync(int pojectId, string userId);
    }
}
