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
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context; 
        public ProjectRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task AddProjectAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p=>p.Id==projectId);

            _context.Projects.Remove(project);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id);
        }

        public IQueryable<Project> GetAllProjectsByUserId(string userId)
        {
            return _context.Projects
                .Where(p => p.ProjectUsers
                .Any(u => u.UserId == userId))
                .AsQueryable();
        }

        public IQueryable<Project> GetAllPublicProjectsByName(string projectName)
        {
            return _context.Projects
                .Where(p => p.Name.Contains(projectName) && p.Visibility == ProjectVisibility.Public)
                .AsQueryable();
        }

        public async Task<Project?> GetByProjectIdAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.ProjectUsers)
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public void UpdateProject(Project project)
        {
            _context.Projects.Update(project);
        }
    }
}
