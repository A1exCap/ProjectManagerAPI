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
        ProjectUserRole EnsureRoleIsValid(string roleName);
        Task EnsureUserIsProjectMemberAsync(int projectId, string userId);
        Task EnsureProjectExistsAsync(int projectId);
        Task EnsureTaskBelongsToProjectAsync(int taskId, int projectId);
        Task EnsureCommentBelongsToTaskAsync(int commentId, int taskId);
        Task EnsureAttachmentBelongsToTaskAsync(int attachmentId, int taskId);
        Task EnsureDocumentBelongsToProjectAsync(int documentId, int projectId);
    }
}
