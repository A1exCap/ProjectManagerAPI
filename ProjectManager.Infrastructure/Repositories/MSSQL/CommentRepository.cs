using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.DTOs.Comment;
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
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public async Task DeleteCommentByIdAsync(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            _context.Comments.Remove(comment);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id);
        }

        public IQueryable<Comment> GetAllCommentsByTaskId(int projectTaskId)
        { 
            return _context.Comments
                .Where(c => c.TaskId == projectTaskId)
                .AsQueryable();
        }



        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c=>c.Id==id);
        }

        public void UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
        }
    }
}
