using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery
{
    public class GetAllCommentsByTaskIdQueryHandler : IRequestHandler<GetAllCommentsByTaskIdQuery, PagedResult<CommentDto>>
    {
        private readonly ILogger<GetAllCommentsByTaskIdQueryHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectAccessService _accessService;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ICommentRepository _commentRepository;
        public GetAllCommentsByTaskIdQueryHandler(ILogger<GetAllCommentsByTaskIdQueryHandler> logger, IProjectRepository projectRepository,
            IProjectAccessService accessService, IProjectTaskRepository projectTaskRepository, ICommentRepository commentRepository)
        {
            _projectRepository = projectRepository;
            _accessService = accessService;
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
            _commentRepository = commentRepository;
        }
        public async Task<PagedResult<CommentDto>> Handle(GetAllCommentsByTaskIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllCommentsByTaskIdQuery by taskId: {TaskId}", request.TaskId);

            var projectExists = await _projectRepository.ExistsAsync(request.ProjectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.ProjectId);
                throw new NotFoundException($"Project with ID {request.ProjectId} does not exist.");
            }

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.TaskId);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exist", request.TaskId);
                throw new NotFoundException($"Task with ID {request.TaskId} does not exist.");
            }

            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var query = _commentRepository.GetAllCommentsByTaskId(request.TaskId);

            var totalCount = await query.CountAsync(cancellationToken);

            var comments = await query.OrderBy(c => c.CreatedAt)
                .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
                .Take(request.QueryParams.PageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} comments by taskId: {TaskId}", totalCount, request.TaskId);
            return new PagedResult<CommentDto>
            {
                Items = comments.Select(CommentMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
