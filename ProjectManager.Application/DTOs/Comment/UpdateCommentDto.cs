using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Comment
{
    public record UpdateCommentDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; init; } = string.Empty;
    }
}
