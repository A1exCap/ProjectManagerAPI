using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class TaskDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; 
        public long FileSize { get; set; }
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;    
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int ProjectTaskId { get; set; }
        public ProjectTask ProjectTask { get; set; } = null!;
        public string? UploadedById { get; set; }
        public User? UploadedBy { get; set; }
    }
}
