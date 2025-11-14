using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectManager.Domain.Entities
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectTaskPriority Priority { get; set; } 
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.ToDo;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }  
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public string? Tags { get; set; } 

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string? AssigneeId { get; set; }
        public User? Assignee { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<TaskDocument> Attachments { get; set; } = new List<TaskDocument>();
    }
    public enum ProjectTaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
    public enum ProjectTaskStatus
    {
        ToDo,
        InProgress,
        CodeReview,
        Testing,
        Done
    }
}
