using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Repositories.MSSQL
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTaskAsync(ProjectTask task)
        {
            await _context.ProjectTasks.AddAsync(task);
        }

        public async Task DeleteTaskByIdAsync(int taskId)
        {
            var task = await GetTaskByIdAsync(taskId);

            if(task == null)
                return;

            _context.ProjectTasks.Remove(task);
        }

        public async Task<bool> ExistsAsync(int taskId)
        {
            return await _context.ProjectTasks.AnyAsync(t => t.Id == taskId);
        }

        public async Task<ICollection<ProjectTask>?> GetAllTasksByProjectIdAsync(int projectId)
        { 
            return await _context.ProjectTasks.Where(t => t.ProjectId == projectId).ToListAsync();
        }

        public async Task<ProjectTask?> GetTaskByIdAsync(int taskId)
        {
            var task = await _context.ProjectTasks
           .Include(t => t.Assignee)
           .Include(t => t.Comments)
               .ThenInclude(c => c.Author)
           .Include(t => t.Attachments)
           .FirstOrDefaultAsync(t => t.Id == taskId);

            if(task == null)
            {
                return null;
            }

            return task;
        }

        public async Task UpdateTaskAsync(ProjectTask task)
        {
           _context.ProjectTasks.Update(task);
        }
    }
}
