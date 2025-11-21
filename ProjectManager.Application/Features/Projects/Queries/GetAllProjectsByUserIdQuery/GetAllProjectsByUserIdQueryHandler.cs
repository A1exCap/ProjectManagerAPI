using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
namespace ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery
{
    public class GetAllProjectsByUserIdQueryHandler : IRequestHandler<GetAllProjectsByUserIdQuery, PagedResult<ProjectDto>>
    {
        private readonly ILogger<GetAllProjectsByUserIdQueryHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        public GetAllProjectsByUserIdQueryHandler(ILogger<GetAllProjectsByUserIdQueryHandler> logger, IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }
        public async Task<PagedResult<ProjectDto>> Handle(GetAllProjectsByUserIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllProjectsByUserIdQuery by userId: {UserId}", request.UserId);

            var query = _projectRepository.GetAllProjectsByUserId(request.UserId);

            var totalCount = await query.CountAsync(cancellationToken);

            var projects = await query.OrderBy(p => p.Status)
              .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
              .Take(request.QueryParams.PageSize)
              .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} projects by userId: {UserId}", totalCount, request.UserId);

            return new PagedResult<ProjectDto>
            {
                Items = projects.Select(ProjectMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
