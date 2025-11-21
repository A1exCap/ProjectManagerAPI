using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Queries.GetAllPublicProjectsByNameQuery
{
    public class GetAllPublicProjectsByNameQueryHandler : IRequestHandler<GetAllPublicProjectsByNameQuery, PagedResult<ProjectDto>>
    {
        private readonly ILogger<GetAllPublicProjectsByNameQueryHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        public GetAllPublicProjectsByNameQueryHandler(ILogger<GetAllPublicProjectsByNameQueryHandler> logger, IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }
        public async Task<PagedResult<ProjectDto>> Handle(GetAllPublicProjectsByNameQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllPublicProjectsByNameQuery by projectName: {ProjectName}", request.ProjectName);

            var query = _projectRepository.GetAllPublicProjectsByName(request.ProjectName);

            var totalCount = await query.CountAsync(cancellationToken);

            var projects = await query.OrderBy(p => p.Status)
              .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
              .Take(request.QueryParams.PageSize)
              .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} projects by projectName: {ProjectId}", totalCount, request.ProjectName);

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
