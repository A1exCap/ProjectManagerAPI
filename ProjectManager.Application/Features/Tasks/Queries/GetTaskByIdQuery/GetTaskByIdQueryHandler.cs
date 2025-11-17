using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, ProjectTaskDetailsDto>
    {
        private readonly IAccessService _accessService;
        private readonly ILogger<GetTaskByIdQueryHandler> _logger;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IEntityValidationService _entityValidationService;
        public GetTaskByIdQueryHandler(IProjectTaskRepository projectTaskRepository, ILogger<GetTaskByIdQueryHandler> logger, 
            IAccessService accessService, IEntityValidationService entityValidationService)
        {
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<ProjectTaskDetailsDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTaskByIdQuery by tsak Id: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);

            _logger.LogInformation("Retrieved task by task id: {TaskId}", request.TaskId);
            return ProjectTaskMapper.ToDetailDto(task);
        }
    }
}
