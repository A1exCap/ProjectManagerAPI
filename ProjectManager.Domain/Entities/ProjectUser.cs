using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string? UserId { get; set; } 
        public ProjectUserRole Role { get; set; } // Viewer, Contributor, Manager, Owner
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public decimal? HourlyRate { get; set; }
        public User? User { get; set; } 
    }
    public enum ProjectUserRole
    {
        Viewer,
        Contributor,
        Manager,
        Owner
    }
}
