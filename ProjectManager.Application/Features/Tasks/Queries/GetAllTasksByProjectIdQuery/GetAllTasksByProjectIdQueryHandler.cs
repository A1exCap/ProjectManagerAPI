using MediatR;
using Microsoft.EntityFrameworkCore;
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

            var query = _projectTaskRepository.GetAllTasksByProjectId(request.ProjectId);

            if (!string.IsNullOrWhiteSpace(request.QueryParams.Title))
                query = query.Where(t => t.Title.Contains(request.QueryParams.Title, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(request.QueryParams.AssigneeEmail))
                query = query.Where(t => t.Assignee != null && t.Assignee.Email == request.QueryParams.AssigneeEmail);

            if (request.QueryParams.Status.HasValue)
                query = query.Where(t => t.Status == request.QueryParams.Status.Value);

            if (request.QueryParams.Priority.HasValue)
                query = query.Where(t => t.Priority == request.QueryParams.Priority.Value);

            query = request.QueryParams.SortBy?.ToLower() switch
            {
                "title" => request.QueryParams.SortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "duedate" => request.QueryParams.SortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "priority" => request.QueryParams.SortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                _ => query.OrderBy(t => t.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
                .Take(request.QueryParams.PageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} tasks for projectId: {ProjectId}", totalCount, request.ProjectId);
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
