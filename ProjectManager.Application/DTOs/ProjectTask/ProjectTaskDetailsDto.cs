using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.TaskDocument;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.ProjectTask
{
    public class ProjectTaskDetailsDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public string? Tags { get; set; }

        public string? AssigneeName { get; set; }
        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public ICollection<TaskAttachmentDto> Attachments { get; set; } = new List<TaskAttachmentDto>();
    }
}
