using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> GetByIdAsync(int id);
        Task AddAsync(Message message);
        Task UpdateAsync(Message message);
        Task DeleteAsync(int id);
    }
}
