using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Comment
{
    public record CreateCommentDto
    {
        public int ProjectTaskId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
