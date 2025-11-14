using MediatR;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand(int ProjectId, int TaskId, string UserId, string Title, string? Description, ProjectTaskPriority Priority,
        ProjectTaskStatus Status, DateTime? DueDate, int EstimatedHours, string? Tags, string? AssigneeEmail) : IRequest<Unit>;
}
