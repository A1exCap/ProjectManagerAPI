using MediatR;
using ProjectManager.Application.DTOs.ProjectTask;

namespace ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery
{
    public record class GetTaskByIdQuery(int TaskId, int ProjectId, string UserId) : IRequest<ProjectTaskDetailsDto>;
}
