using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow.AddDays(7);

        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectDocument> UploadedDocuments { get; set; } = new List<ProjectDocument>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<TaskAttachment> UploadedTaskAttachments { get; set; } = new List<TaskAttachment>();
    }
}
