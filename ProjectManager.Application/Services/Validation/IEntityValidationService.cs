using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.Validation
{
    public interface IEntityValidationService
    {
        Task EnsureProjectExistsAsync(int projectId);
        Task<bool> EnsureTaskBelongsToProjectAsync(int taskId, int projectId);
        Task<bool> EnsureCommentBelongsToTaskAsync(int commentId, int taskId);
        Task<bool> EnsureAttachmentBelongsToTaskAsync(int attachmentId, int taskId);
        Task<bool> EnsureDocumentBelongsToProjectAsync(int documentId, int projectId);
    }
}
