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
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id);
        }

        public async Task<Project?> GetByProjectIdAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.ProjectUsers)
                .Include(p => p.Tasks)
                .Include(p => p.Tasks)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }
    }
}
