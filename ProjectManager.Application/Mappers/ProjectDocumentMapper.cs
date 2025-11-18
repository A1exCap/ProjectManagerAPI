using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Mappers
{
    public static class ProjectDocumentMapper
    {
        public static ProjectDocumentDto ToDto(ProjectDocument document)
        {
            return new ProjectDocumentDto
            {
                Name = document.Name,
                FileSize = document.FileSize,
                ContentType = document.ContentType,
                UploadedAt = document.UploadedAt
            };
        }
    }
}
