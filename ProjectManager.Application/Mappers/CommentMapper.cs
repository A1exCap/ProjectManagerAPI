using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToDto(Comment comment)
        {
            return new CommentDto
            {
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Edited = comment.Edited,
                AuthorName = comment.Author?.UserName
            };
        }
    }
}
