using Microsoft.EntityFrameworkCore;
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
        public async Task<bool> ExistsAsync(int pojectId, string userId)
        {
            return await _context.ProjectUser.AnyAsync(pu => pu.ProjectId == pojectId && pu.UserId == userId);
        }

        public async Task<string> GetUserRoleAsync(int projectId, string userId)
        {
            var role = await _context.ProjectUser
                .Where(pu => pu.ProjectId == projectId && pu.UserId == userId)
                .Select(pu => pu.Role)
                .FirstOrDefaultAsync();

            return role.ToString();
        }
    }
}
