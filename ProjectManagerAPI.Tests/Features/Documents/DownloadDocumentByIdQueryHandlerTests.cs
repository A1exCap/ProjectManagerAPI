using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.ProjectDocuments.Queries.DownloadDocumentByIdQuery;
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
    public class DownloadDocumentByIdQueryHandlerTests
    {
        private readonly ILogger<DownloadDocumentByIdQueryHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IProjectDocumentRepository _projectDocumentRepository;

        private readonly DownloadDocumentByIdQueryHandler _handler;

        public DownloadDocumentByIdQueryHandlerTests()
        {
            _logger = NullLogger<DownloadDocumentByIdQueryHandler>.Instance;

            _blobStorage = A.Fake<IBlobStorageService>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _projectDocumentRepository = A.Fake<IProjectDocumentRepository>();

            _handler = new DownloadDocumentByIdQueryHandler(
                _blobStorage,
                _entityValidationService,
                _accessService,
                _logger,
                _projectDocumentRepository
            );
        }

        [Fact]
        public async Task Handle_ShouldDownloadDocument()
        {
            // Arrange

            var projectId = 1;
            var documentId = 1;
            var userId = "user-123";

            var fileContentString = "Fake PDF content";
            var fileBytes = Encoding.UTF8.GetBytes(fileContentString);
            var fakeStream = new MemoryStream(fileBytes);

            var fakeDocument = new ProjectDocument
            {
                Id = documentId,
                ProjectId = projectId,
                Name = "MyReport.pdf",           
                StoredFileName = "guid-123.pdf", 
                ContentType = "application/pdf"
            };

            A.CallTo(() => _projectDocumentRepository.GetDocumentByIdAsync(documentId))
                .Returns(fakeDocument);

            A.CallTo(() => _blobStorage.DownloadFileAsync("project-documents", fakeDocument.StoredFileName, CancellationToken.None))
                .Returns(fakeStream);

            var query = new DownloadDocumentByIdQuery(projectId, documentId, userId);

            // Act

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert

            result.Should().NotBeNull();
            result.FileName.Should().Be("MyReport.pdf");
            result.ContentType.Should().Be("application/pdf");
            result.FileContent.Should().BeEquivalentTo(fileBytes);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
              .MustHaveHappenedOnceExactly()
              .Then(A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _entityValidationService.EnsureDocumentBelongsToProjectAsync(documentId, projectId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _projectDocumentRepository.GetDocumentByIdAsync(documentId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _blobStorage.DownloadFileAsync("project-documents", "guid-123.pdf", CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
