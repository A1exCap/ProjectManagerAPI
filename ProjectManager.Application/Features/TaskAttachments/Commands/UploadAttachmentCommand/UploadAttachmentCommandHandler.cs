using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.TaskAttachments.Commands.UploadTaskAttachmentCommand
{
    public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, int>
    {
        private readonly ILogger<UploadAttachmentCommandHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        public UploadAttachmentCommandHandler(ILogger<UploadAttachmentCommandHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, IBlobStorageService blobStorage, IUnitOfWork unitOfWork, ITaskAttachmentRepository taskAttachmentRepository)
        {
            _blobStorage = blobStorage;
            _accessService = accessService;
            _entityValidationService = entityValidationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _taskAttachmentRepository = taskAttachmentRepository;
        }
        public async Task<int> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UploadTaskAttachmentCommand for TaskId: {TaskId}", request.TaskId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Contributor", "Manager", "Owner"]);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            var containerName = "task-attachments";

            using var stream = request.File.OpenReadStream();
            var blobUrl = await _blobStorage.UploadFileAsync(containerName, storedFileName, stream, cancellationToken);

            var attachment = new TaskAttachment
            {
                FileName = request.File.FileName,
                StoredFileName = storedFileName,
                FilePath = blobUrl,
                FileSize = request.File.Length,
                ContentType = request.File.ContentType,
                ProjectTaskId = request.TaskId,
                UploadedById = request.UserId,
            };

            await _taskAttachmentRepository.AddAttachmentAsync(attachment); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Uploaded file with ID {AttachmentId} for TaskId: {TaskId}", attachment.Id, request.TaskId);
            return attachment.Id;
        }
    }
}
