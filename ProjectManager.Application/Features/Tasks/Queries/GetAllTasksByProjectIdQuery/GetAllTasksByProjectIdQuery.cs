using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Task;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public record GetAllTasksByProjectIdQuery(int ProjectId, string UserId, TaskQueryParams QueryParams) : IRequest<PagedResult<ProjectTaskDto>>;
}
