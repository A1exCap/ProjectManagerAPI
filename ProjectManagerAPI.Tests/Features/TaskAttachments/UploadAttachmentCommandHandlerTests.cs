using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.TaskAttachments.Commands.UploadTaskAttachmentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.TaskAttachments
{
    public class UploadAttachmentCommandHandlerTests
    {
        private readonly ILogger<UploadAttachmentCommandHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;

        private readonly UploadAttachmentCommandHandler _handler;

        public UploadAttachmentCommandHandlerTests()
        {
            _logger = NullLogger<UploadAttachmentCommandHandler>.Instance;
            _blobStorage = A.Fake<IBlobStorageService>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _accessService = A.Fake<IAccessService>();
            _taskAttachmentRepository = A.Fake<ITaskAttachmentRepository>();

            _handler = new UploadAttachmentCommandHandler(
                _logger,
                _accessService,
                _entityValidationService,
                _blobStorage,
                _unitOfWork,
                _taskAttachmentRepository
            );
        }

        [Fact]
        public async Task Handle_ShouldUploadAttachment()
        {
            // Arrange

            int taskId = 1; 
            var fileFake = A.Fake<IFormFile>();
            string userId = "user-123";
            int projectId = 1;

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake content"));

            A.CallTo(() => fileFake.FileName).Returns("test-doc.pdf");
            A.CallTo(() => fileFake.Length).Returns(stream.Length);
            A.CallTo(() => fileFake.ContentType).Returns("application/pdf");
            A.CallTo(() => fileFake.OpenReadStream()).Returns(stream);

            A.CallTo(() => _blobStorage.UploadFileAsync(A<string>._, A<string>._, A<Stream>._, A<CancellationToken>._))
            .Returns("https://azure.blob/project-documents/guid-test-doc.pdf");

            A.CallTo(() => _taskAttachmentRepository.AddAttachmentAsync(A<TaskAttachment>._))
           .Invokes((TaskAttachment a) => a.Id = 100)
           .Returns(Task.CompletedTask);

            var command = new UploadAttachmentCommand(taskId, fileFake, userId, projectId);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            result.Should().Be(100);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
             .MustHaveHappenedOnceExactly()
             .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Contributor", "Manager", "Owner" }))).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _blobStorage.UploadFileAsync("task-attachments", A<string>.That.EndsWith(".pdf"), A<Stream>._, CancellationToken.None)).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _taskAttachmentRepository.AddAttachmentAsync(A<TaskAttachment>.That.Matches(a => 
                  a.FileName == "test-doc.pdf" &&
                  a.FilePath == "https://azure.blob/project-documents/guid-test-doc.pdf" &&
                  a.ProjectTaskId == taskId))).MustHaveHappenedOnceExactly())
             .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
