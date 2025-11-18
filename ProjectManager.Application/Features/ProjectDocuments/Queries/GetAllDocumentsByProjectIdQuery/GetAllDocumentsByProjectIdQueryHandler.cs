using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery
{
    public class GetAllDocumentsByProjectIdQueryHandler : IRequestHandler<GetAllDocumentsByProjectIdQuery, PagedResult<ProjectDocumentDto>>
    {
        private readonly ILogger<GetAllDocumentsByProjectIdQueryHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IProjectDocumentRepository _projectDocumentRepository;
        public GetAllDocumentsByProjectIdQueryHandler(ILogger<GetAllDocumentsByProjectIdQueryHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, IProjectDocumentRepository projectDocumentRepository)
        {
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _logger = logger;
            _projectDocumentRepository = projectDocumentRepository;
        }
        public async Task<PagedResult<ProjectDocumentDto>> Handle(GetAllDocumentsByProjectIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllDocumentsByProjectIdQuery by projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var query = _projectDocumentRepository.GetAllDocumentsByProjectId(request.ProjectId);

            var totalCount = await query.CountAsync(cancellationToken);

            var documents = await query.OrderBy(a => a.UploadedAt)
              .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
              .Take(request.QueryParams.PageSize)
              .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} documents by projectId: {ProjectId}", totalCount, request.ProjectId);

            return new PagedResult<ProjectDocumentDto>
            {
                Items = documents.Select(ProjectDocumentMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
