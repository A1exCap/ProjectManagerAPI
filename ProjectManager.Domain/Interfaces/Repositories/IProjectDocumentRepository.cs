using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectDocumentRepository
    {
        Task<ProjectDocument> GetByIdAsync(int id);
        Task AddAsync(ProjectDocument projectDocument);
        Task UpdateAsync(ProjectDocument projectDocument);
        Task DeleteAsync(int id);
    }
}
