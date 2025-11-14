using MediatR;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(int ProjectId, string UserId, string Title, string? Description, ProjectTaskPriority Priority, DateTime? DueDate,
      int EstimatedHours, string? Tags, string? AssigneeEmail) : IRequest<int>;
}
