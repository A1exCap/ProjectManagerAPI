using MediatR;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(int ProjectId, string Title, string? Description, ProjectTaskPriority Priority, DateTime? DueDate,
      int EstimatedHours, int ActualHours, string? Tags, string? AssigneeEmail) : IRequest<int>;
}
