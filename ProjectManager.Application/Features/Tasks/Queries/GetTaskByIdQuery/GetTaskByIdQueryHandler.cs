using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, ProjectTaskDetailsDto>
    {
        private readonly IProjectAccessService _accessService;
        private readonly ILogger<GetTaskByIdQueryHandler> _logger;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectRepository _projectRepository;
        public GetTaskByIdQueryHandler(IProjectTaskRepository projectTaskRepository, ILogger<GetTaskByIdQueryHandler> logger, 
            IProjectAccessService accessService, IProjectRepository projectRepository)
        {
            _accessService = accessService;
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
            _projectRepository = projectRepository;
        }
        public async Task<ProjectTaskDetailsDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTaskByIdQuery by tsak Id: {TaskId}", request.TaskId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exists", request.TaskId);
                throw new NotFoundException($"Task with ID {request.TaskId} does not exist.");
            }

            if (task.ProjectId != request.ProjectId)
            {
                _logger.LogWarning("Task with ID {TaskId} does not  belong to project {PojectId}", request.TaskId, request.ProjectId);
                throw new NotFoundException($"Task {request.TaskId} does not belong to project {request.ProjectId}.");
            }

            await _accessService.EnsureUserHasAccessAsync(task.ProjectId, request.UserId);
            _logger.LogInformation("Retrieved task by task id: {TaskId}", request.TaskId);
            return ProjectTaskMapper.ToDetailDto(task);
        }
    }
}
