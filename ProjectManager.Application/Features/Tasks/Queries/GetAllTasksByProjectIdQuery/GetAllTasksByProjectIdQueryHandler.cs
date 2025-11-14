using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public class GetAllTasksByProjectIdQueryHandler : IRequestHandler<GetAllTasksByProjectIdQuery, PagedResult<ProjectTaskDto>>
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

        public async Task<PagedResult<ProjectTaskDto>> Handle(GetAllTasksByProjectIdQuery request, CancellationToken cancellationToken)
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

            if (!string.IsNullOrWhiteSpace(request.QueryParams.Title))
                tasks = tasks.Where(t => t.Title.Contains(request.QueryParams.Title, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(request.QueryParams.AssigneeEmail))
                tasks = tasks.Where(t => t.Assignee != null && t.Assignee.Email == request.QueryParams.AssigneeEmail).ToList();

            if (request.QueryParams.Status.HasValue)
                tasks = tasks.Where(t => t.Status == request.QueryParams.Status.Value).ToList();

            if (request.QueryParams.Priority.HasValue)
                tasks = tasks.Where(t => t.Priority == request.QueryParams.Priority.Value).ToList();

            tasks = request.QueryParams.SortBy?.ToLower() switch
            {
                "title" => request.QueryParams.SortDescending ? tasks.OrderByDescending(t => t.Title).ToList() : tasks.OrderBy(t => t.Title).ToList(),
                "duedate" => request.QueryParams.SortDescending ? tasks.OrderByDescending(t => t.DueDate).ToList() : tasks.OrderBy(t => t.DueDate).ToList(),
                "priority" => request.QueryParams.SortDescending ? tasks.OrderByDescending(t => t.Priority).ToList() : tasks.OrderBy(t => t.Priority).ToList(),
                _ => tasks.OrderBy(t => t.Id).ToList() 
            };

            var totalCount = tasks.Count;
            var items = tasks
                .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
                .Take(request.QueryParams.PageSize)
                .ToList();

            _logger.LogInformation("Retrieved {Count} tasks for projectId: {ProjectId}", tasks.Count, request.ProjectId);
            return new PagedResult<ProjectTaskDto>
            {
                Items = items.Select(ProjectTaskMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
