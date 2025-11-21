using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Project;

namespace ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery
{
    public record GetAllProjectsByUserIdQuery(string UserId, ProjectQueryParams QueryParams) : IRequest<PagedResult<ProjectDto>>;
}
