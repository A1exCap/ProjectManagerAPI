using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.TaskAttachments.Commands.DeleteAttachmentCommand
{
    public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Unit>
    {
        private readonly ILogger<DeleteAttachmentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IBlobStorageService _blobStorage;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAttachmentCommandHandler(ILogger<DeleteAttachmentCommandHandler> logger, IEntityValidationService entityValidationService,
            IAccessService accessService, IBlobStorageService blobStorage, ITaskAttachmentRepository taskAttachmentRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _blobStorage = blobStorage;
            _taskAttachmentRepository = taskAttachmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteAttachmentCommand for AttachmentId: {AttachmentId}", request.AttachmentId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureTaskBelongsToProjectAsync(request.TaskId, request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId,request.UserId,["Contributor", "Manager", "Owner"]);
            await _entityValidationService.EnsureAttachmentBelongsToTaskAsync(request.AttachmentId, request.TaskId);

            var attachment = await _taskAttachmentRepository.GetAttachmentByIdAsync(request.AttachmentId);

            await _blobStorage.DeleteFileAsync("task-attachments",attachment.StoredFileName,cancellationToken);

            await _taskAttachmentRepository.DeleteAttachmentByIdAsync(request.AttachmentId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attachment deleted successfully: {AttachmentId}", request.AttachmentId);

            return Unit.Value;
        }
    }
}
