using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Mappers
{
    public static class ProjectMapper 
    {
        public static ProjectDto ToDto(Project project)
        {
            return new ProjectDto
            {
                Name = project.Name,
                StartDate = project.StartDate,
                Status = project.Status,
                Visibility = project.Visibility
            };
        }

        public static ProjectDetailsDto ToDetailDto(Project project)
        {
            return new ProjectDetailsDto
            {
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Visibility = project.Visibility,
                ClientName = project.ClientName,
                Budget = project.Budget,
                Technologies = project.Technologies,
                OwnerId = project.OwnerId,

                Tasks = project.Tasks.Select(ProjectTaskMapper.ToDto).ToList(),
                ProjectUsers = project.ProjectUsers.Select(ProjectUserMapper.ToDto).ToList(),
                Documents = project.Documents.Select(ProjectDocumentMapper.ToDto).ToList()
            };
        }
    }
}
