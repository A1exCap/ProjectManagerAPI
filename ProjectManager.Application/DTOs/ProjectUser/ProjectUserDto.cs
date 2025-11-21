using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.ProjectUser
{
    public record ProjectUserDto
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;
        [Required]
        public string? UserName { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}
