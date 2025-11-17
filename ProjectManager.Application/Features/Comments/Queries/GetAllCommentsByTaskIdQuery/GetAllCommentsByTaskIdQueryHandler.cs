using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
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
        private readonly IAccessService _accessService;
        private readonly ICommentRepository _commentRepository;
        private readonly IEntityValidationService _entityValidationService;
        public GetAllCommentsByTaskIdQueryHandler(ILogger<GetAllCommentsByTaskIdQueryHandler> logger, IAccessService accessService, 
            ICommentRepository commentRepository, IEntityValidationService entityValidationService)
        {
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _logger = logger;
            _commentRepository = commentRepository;
        }
        public async Task<PagedResult<CommentDto>> Handle(GetAllCommentsByTaskIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllCommentsByTaskIdQuery by taskId: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
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
