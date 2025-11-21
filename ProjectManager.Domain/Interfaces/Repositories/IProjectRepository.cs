using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        void UpdateProject(Project project);
        Task DeleteProjectAsync(int projectId);
        Task<bool> ExistsAsync(int id);
        Task AddProjectAsync(Project project);
        IQueryable<Project> GetAllPublicProjectsByName(string projectName);
        IQueryable<Project> GetAllProjectsByUserId(string userId);
        Task<Project> GetByProjectIdAsync(int projectId);
    }
}
