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
        System.Threading.Tasks.Task AddCommentAsync(Comment comment);
        Task<Comment> GetByIdAsync(int id);
        Task<ICollection<Comment>> GetAllByProjectTaskIdAsync(int projectTaskId);
    }
}
