using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public class GetAllTasksByProjectIdQueryHandler : IRequestHandler<GetAllTasksByProjectIdQuery, ICollection<ProjectTaskDto>>
    {
        private readonly IProjectAccessService _accessService;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<GetAllTasksByProjectIdQueryHandler> _logger;
        public GetAllTasksByProjectIdQueryHandler(IProjectTaskRepository projectTaskRepository, IProjectRepository projectRepository, 
            IProjectAccessService accessService, ILogger<GetAllTasksByProjectIdQueryHandler> logger)
        {
            _logger = logger;
            _accessService = accessService;
            _projectTaskRepository = projectTaskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<ICollection<ProjectTaskDto>> Handle(GetAllTasksByProjectIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllTasksByProjectIdQuery for projectId: {ProjectId}", request.ProjectId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var tasks = await _projectTaskRepository.GetAllTasksByProjectIdAsync(request.ProjectId);
            _logger.LogInformation("Retrieved {Count} tasks for projectId: {ProjectId}", tasks.Count, request.ProjectId);

            return tasks.Select(ProjectTaskMapper.ToDto).ToList();
        }
    }
}
