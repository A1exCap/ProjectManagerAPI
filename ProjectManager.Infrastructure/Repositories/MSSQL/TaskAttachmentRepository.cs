using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
    public class TaskAttachmentRepository : ITaskAttachmentRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskAttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAttachmentAsync(TaskAttachment attachment)
        {
            await _context.TaskAttachments.AddAsync(attachment);
        }

        public async Task DeleteAttachmentByIdAsync(int attachmentId)
        {
            var attachment = await _context.TaskAttachments.FirstOrDefaultAsync(a=>a.Id == attachmentId);
            _context.TaskAttachments.Remove(attachment);
        }

        public IQueryable<TaskAttachment> GetAllAttachmentsByTaskId(int taskId)
        {
            return _context.TaskAttachments
               .Where(a => a.ProjectTaskId == taskId)
               .AsQueryable();
        }

        public async Task<TaskAttachment?> GetAttachmentByIdAsync(int attachmentId)
        {
            return await _context.TaskAttachments.Include(a=>a.ProjectTask).
                Include(a=>a.UploadedBy).FirstOrDefaultAsync(a => a.Id == attachmentId);
        }
    }
}
