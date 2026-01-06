using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.ProjectDocuments.Commands.DeleteDocumentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Documents
{
    public class DeleteDocumentCommandHandlerTests
    {
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IBlobStorageService _blobStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectDocumentRepository _projectDocumentRepository;

        private readonly DeleteDocumentCommandHandler _handler;

        public DeleteDocumentCommandHandlerTests()
        {
            _logger = NullLogger<DeleteDocumentCommandHandler>.Instance;

            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _blobStorage = A.Fake<IBlobStorageService>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _projectDocumentRepository = A.Fake<IProjectDocumentRepository>();

            _handler = new DeleteDocumentCommandHandler(
                _logger,
                _entityValidationService,
                _accessService,
                _blobStorage,
                _unitOfWork,
                _projectDocumentRepository
            );
        }

        [Fact]
        public async Task Handle_ShouldDeleteDocument()
        {
            // Arrange

            var projectId = 1;
            var userId = "user-123";
            var documentId = 1;

            var fakeDocument = new ProjectDocument
            {
                Id = documentId,
                ProjectId = projectId,
                StoredFileName = "guid-test-file.pdf" 
            };

            A.CallTo(() => _projectDocumentRepository.GetDocumentByIdAsync(documentId))
                .Returns(fakeDocument);

            var command = new DeleteDocumentCommand(projectId, userId, documentId);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
              .MustHaveHappenedOnceExactly()
              .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Contributor", "Manager", "Owner" }))).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _entityValidationService.EnsureDocumentBelongsToProjectAsync(documentId, projectId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _projectDocumentRepository.GetDocumentByIdAsync(documentId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _blobStorage.DeleteFileAsync("project-documents", "guid-test-file.pdf", CancellationToken.None)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _projectDocumentRepository.DeleteDocumentByIdAsync(documentId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
