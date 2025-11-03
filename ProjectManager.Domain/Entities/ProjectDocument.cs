using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class ProjectDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;    
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string? UploadedById { get; set; }
        public User? UploadedBy { get; set; } 
    }
}
