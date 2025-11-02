using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectTaskRepository
    {
        Task<ProjectTask> GetByIdAsync(int id);
        Task AddAsync(ProjectTask projectTask);
        Task UpdateAsync(ProjectTask projectTask);
        Task DeleteAsync(int id);
    }
}
