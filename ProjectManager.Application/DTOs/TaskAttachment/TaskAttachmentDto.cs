using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.TaskDocument
{
    public record TaskAttachmentDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string ContentType { get; set; } = string.Empty;
        [Required]
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
