using ProjectManager.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Application.DTOs.Task
{
    public record ProjectTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public ProjectTaskPriority Priority { get; set; }
        [Required]
        public ProjectTaskStatus Status { get; set; }
    }
}
