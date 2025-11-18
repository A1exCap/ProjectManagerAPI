using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.TaskAttachment;
using ProjectManager.Application.Features.TaskAttachments.Queries.DownloadAttachmentByIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Queries.DownloadDocumentByIdQuery
{
    public class DownloadDocumentByIdQueryHandler : IRequestHandler<DownloadDocumentByIdQuery, DownloadDocumentDto>
    {
        private readonly ILogger<DownloadDocumentByIdQueryHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IProjectDocumentRepository _projectDocumentRepository;

        public DownloadDocumentByIdQueryHandler(IBlobStorageService blobStorage, IEntityValidationService entityValidationService,
            IAccessService accessService, ILogger<DownloadDocumentByIdQueryHandler> logger, IProjectDocumentRepository projectDocumentRepository)
        {
            _logger = logger;
            _blobStorage = blobStorage;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _projectDocumentRepository = projectDocumentRepository;
        }
        public async Task<DownloadDocumentDto> Handle(DownloadDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DownloadDocumentByIdQuery by documentId: {DocumentId}", request.DocumentId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserHasAccessAsync(request.ProjectId, request.UserId);
            await _entityValidationService.EnsureDocumentBelongsToProjectAsync(request.DocumentId, request.ProjectId);

            var document = await _projectDocumentRepository.GetDocumentByIdAsync(request.DocumentId);

            using var stream = await _blobStorage.DownloadFileAsync("project-documents", document.StoredFileName, cancellationToken);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            _logger.LogInformation("Downloaded document with ID {DocumentId}", document.Id);

            return new DownloadDocumentDto
            {
                FileContent = memoryStream.ToArray(),
                FileName = document.Name,
                ContentType = document.ContentType
            };
        }
    }
}
