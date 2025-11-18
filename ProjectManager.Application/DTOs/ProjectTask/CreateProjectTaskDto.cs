using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.ProjectTask
{
    public record CreateProjectTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public ProjectTaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public string? Tags { get; set; }
        [EmailAddress]
        public string? AssigneeEmail { get; set; }
    }
}
