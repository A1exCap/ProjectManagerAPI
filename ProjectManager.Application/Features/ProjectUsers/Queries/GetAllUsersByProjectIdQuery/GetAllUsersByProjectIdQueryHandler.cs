using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.ProjectUser;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Queries.GetAllUsersByProjectIdQuery
{
    public class GetAllUsersByProjectIdQueryHandler : IRequestHandler<GetAllUsersByProjectIdQuery, PagedResult<ProjectUserDto>>
    {
        private readonly ILogger<GetAllUsersByProjectIdQueryHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IProjectUserRepository _projectUserRepository;
        public GetAllUsersByProjectIdQueryHandler(ILogger<GetAllUsersByProjectIdQueryHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, IProjectUserRepository projectUserRepository)
        {
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _logger = logger;
            _projectUserRepository = projectUserRepository;
        }
        public async Task<PagedResult<ProjectUserDto>> Handle(GetAllUsersByProjectIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllUsersByProjectIdQuery by projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var query = _projectUserRepository.GetAllUsersByProjectId(request.ProjectId);

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query.OrderBy(u => u.JoinedAt)
              .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
              .Take(request.QueryParams.PageSize)
              .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} users by projectId: {ProjectId}", totalCount, request.ProjectId);

            return new PagedResult<ProjectUserDto>
            {
                Items = users.Select(ProjectUserMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
