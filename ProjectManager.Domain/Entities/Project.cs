using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public ProjectStatus Status { get; set; } 
        public ProjectVisibility Visibility { get; set; } 
        public string? ClientName { get; set; }
        public decimal? Budget { get; set; }
        public string? Technologies { get; set; } 
        public string? OwnerId { get; set; }
        public User? Owner { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        public ICollection<ProjectDocument> Documents { get; set; } = new List<ProjectDocument>();
    }
    public enum ProjectStatus
    {
        Planned,
        Active,
        OnHold,
        Completed,
        Cancelled
    }
    public enum ProjectVisibility
    {
        Public,
        Private
    }
}
