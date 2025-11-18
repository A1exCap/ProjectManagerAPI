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
        Task<bool> ExistsAsync(int id);
        Task<Project> GetByProjectIdAsync(int projectId);
    }
}
