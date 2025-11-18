using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.TaskAttachment;
using ProjectManager.Application.DTOs.TaskDocument;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.TaskAttachments.Queries.DownloadAttachmentByIdQuery
{
    public class DownloadAttachmentByIdQueryHandler : IRequestHandler<DownloadAttachmentByIdQuery, DownloadTaskAttachmentDto>
    {
        private readonly ILogger<DownloadAttachmentByIdQueryHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;

        public DownloadAttachmentByIdQueryHandler(IBlobStorageService blobStorage,IEntityValidationService entityValidationService,
            IAccessService accessService, ITaskAttachmentRepository taskAttachmentRepository, ILogger<DownloadAttachmentByIdQueryHandler> logger)
        {
            _logger = logger;
            _blobStorage = blobStorage;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _taskAttachmentRepository = taskAttachmentRepository;
        }

        public async Task<DownloadTaskAttachmentDto> Handle(DownloadAttachmentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DownloadTaskAttachmentQuery for attachmentId: {AttachmentId}", request.AttachmentId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);
            await _entityValidationService.EnsureAttachmentBelongsToTaskAsync(request.AttachmentId, request.TaskId);

            var attachment = await _taskAttachmentRepository.GetAttachmentByIdAsync(request.AttachmentId);

            using var stream = await _blobStorage.DownloadFileAsync("task-attachments", attachment.StoredFileName, cancellationToken);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            _logger.LogInformation("Downloaded file with ID {AttachmentId}", attachment.Id);

            return new DownloadTaskAttachmentDto
            {
                FileContent = memoryStream.ToArray(),
                FileName = attachment.FileName,
                ContentType = attachment.ContentType
            };
        }
    }
}
