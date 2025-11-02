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
        Task<ProjectUser> GetByIdAsync(int id);
        Task<ProjectUser> GetByProjectAndUserAsync(int projectId, Guid userId);
        Task<List<ProjectUser>> GetByProjectAsync(int projectId);
        Task<List<ProjectUser>> GetByUserAsync(Guid userId);
        Task AddAsync(ProjectUser projectUser);
        Task UpdateAsync(ProjectUser projectUser);
        Task DeleteAsync(int id);

        Task<bool> IsUserInProjectAsync(int projectId, Guid userId);
        Task<ProjectUserRole> GetUserRoleAsync(int projectId, Guid userId);
        Task UpdateUserRoleAsync(int projectId, Guid userId, ProjectUserRole newRole);

        Task<bool> ChangeUserRateAsync(int projectId, Guid userId, decimal newRate);
    }
}
