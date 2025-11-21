using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Project
{
    public record ProjectDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public ProjectStatus Status { get; set; }
        [Required]
        public ProjectVisibility Visibility { get; set; }
    }
}
