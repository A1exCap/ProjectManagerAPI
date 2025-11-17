using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public class GetAllTasksByProjectIdQueryHandler : IRequestHandler<GetAllTasksByProjectIdQuery, PagedResult<ProjectTaskDto>>
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<GetAllTasksByProjectIdQueryHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        public GetAllTasksByProjectIdQueryHandler(IProjectTaskRepository projectTaskRepository, ILogger<GetAllTasksByProjectIdQueryHandler> logger, 
            IEntityValidationService entityValidationService, IAccessService accessService)
        {
            _entityValidationService = entityValidationService;
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
            _accessService = accessService;
        }

        public async Task<PagedResult<ProjectTaskDto>> Handle(GetAllTasksByProjectIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllTasksByProjectIdQuery for projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
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
