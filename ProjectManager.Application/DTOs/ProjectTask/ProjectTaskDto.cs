using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.DTOs.Task
{
    public record ProjectTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public ProjectTaskPriority Priority { get; set; }
        public ProjectTaskStatus Status { get; set; }
    }
}
