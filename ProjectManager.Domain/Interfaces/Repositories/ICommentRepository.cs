using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment comment);
        Task DeleteCommentByIdAsync(int commentId);
        Task<Comment> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);

        IQueryable<Comment> GetAllCommentsByTaskId(int projectTaskId);
        void UpdateComment(Comment comment);
    }
}
