using MediatR;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand(int ProjectId, int TaskId, string UserId, ProjectTaskUpdateDto dto) : IRequest<Unit>;
}
