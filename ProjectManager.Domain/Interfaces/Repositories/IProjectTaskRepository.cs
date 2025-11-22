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
        IQueryable<ProjectTask> GetTasksWithUpcomingDeadlines(TimeSpan threshold);
        IQueryable<ProjectTask> GetAllTasksByProjectId(int projectId);
        Task<ProjectTask> GetTaskByIdAsync(int taskId);
        Task AddTaskAsync(ProjectTask task);
        void UpdateTask(ProjectTask task);
        Task DeleteTaskByIdAsync(int taskId);
    }
}
