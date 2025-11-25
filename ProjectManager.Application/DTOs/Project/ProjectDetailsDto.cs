using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.ProjectUser;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Project
{
    public record ProjectDetailsDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        [Required]
        public ProjectStatus Status { get; set; }
        [Required]
        public ProjectVisibility Visibility { get; set; }
        public string? ClientName { get; set; }
        public decimal? Budget { get; set; }
        public string? Technologies { get; set; }
        public string? OwnerId { get; set; }

        public ICollection<ProjectTaskDto> Tasks { get; set; } = new List<ProjectTaskDto>();
        public ICollection<ProjectUserDto> ProjectUsers { get; set; } = new List<ProjectUserDto>();
        public ICollection<ProjectDocumentDto> Documents { get; set; } = new List<ProjectDocumentDto>();
    }
}
