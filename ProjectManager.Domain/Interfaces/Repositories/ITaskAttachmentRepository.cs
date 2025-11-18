using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface ITaskAttachmentRepository
    {
        IQueryable<TaskAttachment> GetAllAttachmentsByTaskId(int taskId);
        Task AddAttachmentAsync(TaskAttachment attachment);
        Task<TaskAttachment> GetAttachmentByIdAsync(int attachmentId);
        Task DeleteAttachmentByIdAsync(int attachmentId);
    }
}
