using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.TaskAttachments.Commands.DeleteAttachmentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Commands.DeleteDocumentCommand
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
    {
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IBlobStorageService _blobStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectDocumentRepository _projectDocumentRepository;

        public DeleteDocumentCommandHandler(ILogger<DeleteDocumentCommandHandler> logger, IEntityValidationService entityValidationService,
            IAccessService accessService, IBlobStorageService blobStorage, IUnitOfWork unitOfWork, IProjectDocumentRepository projectDocumentRepository)
        {
            _logger = logger;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _blobStorage = blobStorage;
            _unitOfWork = unitOfWork;
            _projectDocumentRepository = projectDocumentRepository;
        }
        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteDocumentCommand by DocumentId: {DocumentId}", request.DocumentId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Contributor", "Manager", "Owner"]);
            await _entityValidationService.EnsureDocumentBelongsToProjectAsync(request.DocumentId, request.ProjectId);

            var document = await _projectDocumentRepository.GetDocumentByIdAsync(request.DocumentId);

            await _blobStorage.DeleteFileAsync("project-documents", document.StoredFileName, cancellationToken);

            await _projectDocumentRepository.DeleteDocumentByIdAsync(request.DocumentId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document deleted successfully: {DocumentId}", request.DocumentId);

            return Unit.Value;
        }
    }
}
