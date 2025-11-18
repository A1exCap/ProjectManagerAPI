using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.TaskDocument;
using ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.TaskAttachments.Queries.GetAllAttachmentsByTaskIdQuery
{
    public class GetAllAttachmentsByTaskIdQueryHandler : IRequestHandler<GetAllAttachmentsByTaskIdQuery, PagedResult<TaskAttachmentDto>>
    {
        private readonly ILogger<GetAllAttachmentsByTaskIdQueryHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        private readonly IAccessService _accessService;
        public GetAllAttachmentsByTaskIdQueryHandler(ILogger<GetAllAttachmentsByTaskIdQueryHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, ITaskAttachmentRepository taskAttachmentRepository)
        {
            _taskAttachmentRepository = taskAttachmentRepository;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _logger = logger;
        }
        public async Task<PagedResult<TaskAttachmentDto>> Handle(GetAllAttachmentsByTaskIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllAttachmentsByTaskIdQuery by taskId: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);

            var query = _taskAttachmentRepository.GetAllAttachmentsByTaskId(request.TaskId);

            var totalCount = await query.CountAsync(cancellationToken);

            var attachments = await query.OrderBy(a => a.UploadedAt)
              .Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
              .Take(request.QueryParams.PageSize)
              .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {TotalCount} attachments by taskId: {TaskId}", totalCount, request.TaskId);

            return new PagedResult<TaskAttachmentDto>
            {
                Items = attachments.Select(TaskAttachmentMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = request.QueryParams.PageNumber,
                PageSize = request.QueryParams.PageSize
            };
        }
    }
}
