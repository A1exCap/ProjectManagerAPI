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
            _context.ProjectTasks.Remove(task);
        }

        public async Task<bool> ExistsAsync(int taskId)
        {
            return await _context.ProjectTasks.AnyAsync(t => t.Id == taskId);
        }

        public IQueryable<ProjectTask> GetAllTasksByProjectId(int projectId)
        {
            return _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .AsQueryable();
        }

        public async Task<ProjectTask?> GetTaskByIdAsync(int taskId)
        {
            return await _context.ProjectTasks
           .Include(t => t.Assignee)
           .Include(t => t.Comments)
               .ThenInclude(c => c.Author)
           .Include(t => t.Attachments)
           .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public void UpdateTask(ProjectTask task)
        {
             _context.ProjectTasks.Update(task);
        }
    }
}
