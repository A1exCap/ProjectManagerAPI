using FakeItEasy;
using FluentAssertions;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.ProjectDocuments.Commands.UploadDocumentCommand;
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
    public class UploadDocumentCommandHandlerTests
    {
        private readonly ILogger<UploadDocumentCommandHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        private readonly IProjectDocumentRepository _projectDocumentRepository;

        private readonly UploadDocumentCommandHandler _handler;

        public UploadDocumentCommandHandlerTests()
        {
            _logger = NullLogger<UploadDocumentCommandHandler>.Instance;
            _blobStorage = A.Fake<IBlobStorageService>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _accessService = A.Fake<IAccessService>();
            _projectDocumentRepository = A.Fake<IProjectDocumentRepository>();

            _handler = new UploadDocumentCommandHandler(
                _logger,
                _accessService,
                _entityValidationService,
                _blobStorage,
                _unitOfWork,
                _projectDocumentRepository
            );
        }

        [Fact]
        public async Task Handle_ShouldUploadDocument()
        {
            // Arrange

            string userId = "user-123";
            var projectId = 1;

            var fileFake = A.Fake<IFormFile>();

            // Також налаштуємо стрім, щоб не впало далі
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake content"));

            A.CallTo(() => fileFake.FileName).Returns("test-doc.pdf");
            A.CallTo(() => fileFake.Length).Returns(stream.Length);
            A.CallTo(() => fileFake.ContentType).Returns("application/pdf");
            A.CallTo(() => fileFake.OpenReadStream()).Returns(stream);

            A.CallTo(() => _blobStorage.UploadFileAsync(A<string>._, A<string>._, A<Stream>._, A<CancellationToken>._))
            .Returns("https://azure.blob/project-documents/guid-test-doc.pdf");

            A.CallTo(() => _projectDocumentRepository.AddDocumentAsync(A<ProjectDocument>._))
            .Invokes((ProjectDocument d) => d.Id = 100)
            .Returns(Task.CompletedTask);

            var command = new UploadDocumentCommand(fileFake, userId, projectId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            result.Should().Be(100);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
             .MustHaveHappenedOnceExactly()
             .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Contributor", "Manager", "Owner" }))).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _blobStorage.UploadFileAsync("project-documents", A<string>.That.EndsWith(".pdf"), A<Stream>._, CancellationToken.None)).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _projectDocumentRepository.AddDocumentAsync(A<ProjectDocument>.That.Matches(d =>
                    d.Name == "test-doc.pdf" &&
                    d.FilePath == "https://azure.blob/project-documents/guid-test-doc.pdf" &&
                    d.ProjectId == projectId
                )
                )).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
