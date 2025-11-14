using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectUserRepository
    {
        Task<string> GetUserRoleAsync(int projectId, string userId);
        Task<bool> ExistsAsync(int pojectId, string userId);
    }
}
