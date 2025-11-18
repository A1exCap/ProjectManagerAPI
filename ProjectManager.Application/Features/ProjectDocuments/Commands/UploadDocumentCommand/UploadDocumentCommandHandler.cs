using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.TaskAttachments.Commands.UploadTaskAttachmentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Commands.UploadDocumentCommand
{
    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, int>
    {
        private readonly ILogger<UploadDocumentCommandHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        private readonly IProjectDocumentRepository _projectDocumentRepository;
        public UploadDocumentCommandHandler(ILogger<UploadDocumentCommandHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, IBlobStorageService blobStorage, IUnitOfWork unitOfWork, IProjectDocumentRepository projectDocumentRepository)
        {
            _blobStorage = blobStorage;
            _accessService = accessService;
            _entityValidationService = entityValidationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _projectDocumentRepository = projectDocumentRepository;
        }

        public async Task<int> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UploadDocumentCommand for projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Contributor", "Manager", "Owner"]);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            var containerName = "project-documents";

            using var stream = request.File.OpenReadStream();
            var blobUrl = await _blobStorage.UploadFileAsync(containerName, storedFileName, stream, cancellationToken);

            var document = new ProjectDocument
            {
                Name = request.File.FileName,
                StoredFileName = storedFileName,
                FilePath = blobUrl,
                FileSize = request.File.Length,
                ContentType = request.File.ContentType,
                ProjectId = request.ProjectId,
                UploadedById = request.UserId
            };

            await _projectDocumentRepository.AddDocumentAsync(document);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Uploaded file with ID {DocumentId} for projectId: {ProjectId}", document.Id, request.ProjectId);
            return document.Id;
        }
    }
}
