using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.TaskAttachments.Queries.DownloadAttachmentByIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.TaskAttachments
{
    public class DownloadAttachmentByIdQueryHandlerTests
    {
        private readonly ILogger<DownloadAttachmentByIdQueryHandler> _logger;
        private readonly IBlobStorageService _blobStorage;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;

        private readonly DownloadAttachmentByIdQueryHandler _handler;

        public DownloadAttachmentByIdQueryHandlerTests()
        {
            _logger = NullLogger<DownloadAttachmentByIdQueryHandler>.Instance;

            _blobStorage = A.Fake<IBlobStorageService>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _taskAttachmentRepository = A.Fake<ITaskAttachmentRepository>();

            _handler = new DownloadAttachmentByIdQueryHandler(
                _blobStorage,
                _entityValidationService,
                _accessService,
                _taskAttachmentRepository,
                _logger
            );
        }

        [Fact]
        public async Task Handle_ShouldDownloadAttachment()
        {
            // Arrange

            var projectId = 1;
            var taskId = 1;
            var attachmentId = 1;
            var userId = "user-123";

            var fileContentString = "Fake PDF content";
            var fileBytes = Encoding.UTF8.GetBytes(fileContentString);
            var fakeStream = new MemoryStream(fileBytes);

            var fakeAttachment = new TaskAttachment
            {
                Id = attachmentId,
                ProjectTaskId = taskId,
                FileName = "MyReport.pdf",
                StoredFileName = "guid-123.pdf",
                ContentType = "application/pdf"
            };

            A.CallTo(() => _taskAttachmentRepository.GetAttachmentByIdAsync(attachmentId))
                .Returns(fakeAttachment);

            A.CallTo(() => _blobStorage.DownloadFileAsync("task-attachments", fakeAttachment.StoredFileName, CancellationToken.None))
                .Returns(fakeStream);

            var query = new DownloadAttachmentByIdQuery(projectId, taskId, attachmentId, userId);

            // Act

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert

            result.Should().NotBeNull();
            result.FileName.Should().Be("MyReport.pdf");
            result.ContentType.Should().Be("application/pdf");
            result.FileContent.Should().BeEquivalentTo(fileBytes);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _entityValidationService.EnsureAttachmentBelongsToTaskAsync(attachmentId, taskId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _taskAttachmentRepository.GetAttachmentByIdAsync(attachmentId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _blobStorage.DownloadFileAsync("task-attachments", fakeAttachment.StoredFileName, CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
