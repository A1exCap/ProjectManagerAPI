using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Comment
{
    public record CommentDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool? Edited { get; set; }
        public string? AuthorName { get; set; } 
    }
}
