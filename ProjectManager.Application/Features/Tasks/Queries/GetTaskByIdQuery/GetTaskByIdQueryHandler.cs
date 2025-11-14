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
        public GetTaskByIdQueryHandler(IProjectTaskRepository projectTaskRepository, ILogger<GetTaskByIdQueryHandler> logger, 
            IProjectAccessService accessService)
        {
            _accessService = accessService;
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<ProjectTaskDetailsDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTaskByIdQuery by tsak Id: {TaskId}", request.TaskId);
            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exists", request.TaskId);
                throw new NotFoundException($"Task with ID {request.TaskId} does not exist.");
            }

            await _accessService.EnsureUserHasAccessAsync(task.ProjectId, request.UserId);
            _logger.LogInformation("Retrieved task by task id: {TaskId}", request.TaskId);
            return ProjectTaskMapper.ToDetailDto(task);
        }
    }
}
