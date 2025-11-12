using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.DTOs.TaskDocument;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Mappers
{
    public static class TaskDocumentMapper
    {
        public static TaskDocumentDto ToDto(TaskDocument taskDoc)
        {
            return new TaskDocumentDto
            {
                FileName = taskDoc.FileName,
                ContentType = taskDoc.ContentType,
                FileSize = taskDoc.FileSize,
                StoredFileName = taskDoc.StoredFileName,
                FilePath = taskDoc.FilePath,
                UploadedAt = taskDoc.UploadedAt
            };
        }
    }
}
