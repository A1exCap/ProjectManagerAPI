using ProjectManager.Application.DTOs.Comment;
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
    public static class ProjectTaskMapper
    {
        public static ProjectTaskDto ToDto(ProjectTask task)
        {
            return new ProjectTaskDto
            {
               Title = task.Title,
               Priority = task.Priority,
               Status = task.Status
            };
        }

        public static ProjectTaskDetailsDto ToDetailDto(ProjectTask task)
        {
            return new ProjectTaskDetailsDto
            {
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                Tags = task.Tags,
                AssigneeName = task.Assignee.UserName,
                Comments = task.Comments.Select(CommentMapper.ToDto).ToList(),
                Attachments = task.Attachments.Select(TaskAttachmentMapper.ToDto).ToList()
            };
        }
    }
}
