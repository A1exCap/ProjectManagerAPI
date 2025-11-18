using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<Message> GetMessageByIdAsync(int messageId);
        void UpdateMessage(Message message);
        IQueryable<Message> GetMessagesByUserId(string userId);
        Task DeleteMessageByIdAsync(int messageId);
    }
}
