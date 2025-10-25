using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class ProjectMember
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int UserId { get; set; }
        public ProjectMemberRole Role { get; set; } // Viewer, Contributor, Manager, Owner
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
    }
    public enum ProjectMemberRole
    {
        Viewer,
        Contributor,
        Manager,
        Owner
    }
}
