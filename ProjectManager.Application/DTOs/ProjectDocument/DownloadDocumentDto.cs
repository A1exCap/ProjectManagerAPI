using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.ProjectDocument
{
    public record DownloadDocumentDto
    {
        [Required]
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
