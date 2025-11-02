using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface ITaskDocumentRepositry
    {
        Task<TaskDocument> GetByIdAsync(int id);
        Task AddAsync(TaskDocument taskDocument);
        Task UpdateAsync(TaskDocument taskDocument);
        Task DeleteAsync(int id);
    }
}
