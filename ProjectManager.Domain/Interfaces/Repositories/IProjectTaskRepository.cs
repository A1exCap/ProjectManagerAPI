using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectTaskRepository
    {
        Task<ICollection<ProjectTask>> GetAllTasksByProjectIdAsync(int projectId);
        Task<ProjectTask> GetTaskByIdAsync(int taskId);
        Task AddTaskAsync(ProjectTask task);
        Task UpdateTaskAsync(ProjectTask task);
        Task DeleteTaskByIdAsync(int taskId);
        Task<bool> ExistsAsync(int taskId);
    }
}
