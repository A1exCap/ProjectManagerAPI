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
    public class ProjectUserRepository : IProjectUserRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddProjectUserAsync(ProjectUser projectUser)
        {
            await _context.ProjectUser.AddAsync(projectUser);
        }

        public void DeleteProjectUser(ProjectUser projectUser)
        {
            _context.ProjectUser.Remove(projectUser);
        }

        public async Task<bool> ExistsAsync(int pojectId, string userId)
        {
            return await _context.ProjectUser.AnyAsync(pu => pu.ProjectId == pojectId && pu.UserId == userId);
        }

        public IQueryable<ProjectUser> GetAllUsersByProjectId(int projectId)
        {
            return _context.ProjectUser
                .Where(t => t.ProjectId == projectId)
                .AsQueryable();
        }

        public async Task<ProjectUser?> GetProjectUserdAsync(int projectId, string userId)
        {
            return await _context.ProjectUser
                .Where(pu => pu.ProjectId == projectId && pu.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetUserRoleAsync(int projectId, string userId)
        {
            var role = await _context.ProjectUser
                .Where(pu => pu.ProjectId == projectId && pu.UserId == userId)
                .Select(pu => pu.Role)
                .FirstOrDefaultAsync();

            return role.ToString();
        }

        public void UpdateProjectUser(ProjectUser projectUser)
        {
            _context.ProjectUser.Update(projectUser);
        }
    }
}
