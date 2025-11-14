using MediatR;
using ProjectManager.Application.DTOs.Task;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public record class GetAllTasksByProjectIdQuery(int ProjectId, string UserId) : IRequest<ICollection<ProjectTaskDto>>;
}
