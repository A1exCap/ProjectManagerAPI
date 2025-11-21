using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Project
{
    public record ProjectUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public ProjectStatus Status { get; set; }
        [Required]
        public ProjectVisibility Visibility { get; set; }
        public string? ClientName { get; set; }
        public decimal? Budget { get; set; }
        public string? Technologies { get; set; }
    }
}
