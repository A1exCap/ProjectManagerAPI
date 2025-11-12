using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Comment
{
    public record UpdateCommentDto
    {
        public string Content { get; init; } = string.Empty;
    }
}
