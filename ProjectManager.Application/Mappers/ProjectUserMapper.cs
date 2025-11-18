using ProjectManager.Application.DTOs.ProjectUser;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Mappers
{
    public static class ProjectUserMapper
    {
        public static ProjectUserDto ToDto(ProjectUser projectUser)
        {
            return new ProjectUserDto
            {
                ProjectName = projectUser.Project.Name,
                UserName = projectUser.User.UserName,
                Role = projectUser.Role.ToString(),
                JoinedAt = projectUser.JoinedAt
            };
        }
    }
}
