using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.ProjectUser
{
    public record AddUserToProjectDto
    {
        [Required]
        public string UserToAddId { get; set; } = string.Empty;
        [Required, EnumDataType(typeof(ProjectUserRole), ErrorMessage = "Invalid user role.")]
        public string UserRole { get; set; } = string.Empty;
    }
}
