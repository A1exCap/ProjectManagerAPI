using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool? Edited{ get; set; }
        public int ProjectTaskId { get; set; }
        public ProjectTask ProjectTask { get; set; } = null!;
        public string? AuthorId { get; set; }
        public User? Author { get; set; } 
    }
}
