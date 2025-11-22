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
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;
        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task DeleteMessageByIdAsync(int messageId)
        {
            var message = await GetMessageByIdAsync(messageId);
            _context.Messages.Remove(message);
        }

        public void DeleteMessagesByUserId(string userId)
        {
            var messages = _context.Messages.Where(m => m.UserId == userId);
            _context.Messages.RemoveRange(messages);
        }

        public async Task<Message?> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages.Include(m=>m.User).FirstOrDefaultAsync(m=>m.Id==messageId);
        }

        public IQueryable<Message> GetMessagesByUserId(string userId)
        {
            return _context.Messages
               .Where(m => m.UserId == userId)
               .AsQueryable();
        }
    }
}
