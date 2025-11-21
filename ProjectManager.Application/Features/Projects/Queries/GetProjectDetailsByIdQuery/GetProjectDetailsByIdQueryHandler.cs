using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Queries.GetAllPublicProjectsByNameQuery;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Queries.GetProjectDetailsByIdQuery
{
    public class GetProjectDetailsByIdQueryHandler : IRequestHandler<GetProjectDetailsByIdQuery, ProjectDetailsDto>
    {
        private readonly ILogger<GetProjectDetailsByIdQueryHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IEntityValidationService _entityValidationService;
        public GetProjectDetailsByIdQueryHandler(ILogger<GetProjectDetailsByIdQueryHandler> logger, IProjectRepository projectRepository,
            IEntityValidationService entityValidationService)
        {
            _entityValidationService = entityValidationService;
            _projectRepository = projectRepository;
            _logger = logger;
        }
        public async Task<ProjectDetailsDto> Handle(GetProjectDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProjectDetailsByIdQueryHandler by projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);

            var project = await _projectRepository.GetByProjectIdAsync(request.ProjectId);

            _logger.LogInformation("Retrieved project by project id: {ProjectId}", request.ProjectId);
            return ProjectMapper.ToDetailDto(project);
        }
    }
}
