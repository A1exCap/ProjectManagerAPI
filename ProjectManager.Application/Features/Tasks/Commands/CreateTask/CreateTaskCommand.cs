using MediatR;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(int ProjectId, string UserId, CreateProjectTaskDto dto) : IRequest<int>;
}
