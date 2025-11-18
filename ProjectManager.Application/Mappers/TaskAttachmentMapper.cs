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
    public static class TaskAttachmentMapper
    {
        public static TaskAttachmentDto ToDto(TaskAttachment taskAttachment)
        {
            return new TaskAttachmentDto
            {
                FileName = taskAttachment.FileName,
                ContentType = taskAttachment.ContentType,
                FileSize = taskAttachment.FileSize,
                UploadedAt = taskAttachment.UploadedAt
            };
        }
    }
}
