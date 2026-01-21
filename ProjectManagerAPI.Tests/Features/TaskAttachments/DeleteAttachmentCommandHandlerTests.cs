using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.ProjectDocuments.Commands.DeleteDocumentCommand;
using ProjectManager.Application.Features.TaskAttachments.Commands.DeleteAttachmentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.TaskAttachments
{
    public class DeleteAttachmentCommandHandlerTests
    {
        private readonly ILogger<DeleteAttachmentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IBlobStorageService _blobStorage;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly DeleteAttachmentCommandHandler _handler;

        public DeleteAttachmentCommandHandlerTests()
        {
            _logger = NullLogger<DeleteAttachmentCommandHandler>.Instance;
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _blobStorage = A.Fake<IBlobStorageService>();
            _taskAttachmentRepository = A.Fake<ITaskAttachmentRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new DeleteAttachmentCommandHandler(
                _logger,
                _entityValidationService,
                _accessService,
                _blobStorage,
                _taskAttachmentRepository,
                _unitOfWork
            );
        }

        [Fact]
        public async Task Handle_ShouldDeleteAttachment_WhenValidationSucceeds()
        {
            // Arrange

            int projectId = 1;
            int taskId = 1;
            string userId = "user-123";
            int attachmentId = 1;
            string blobFileName = "guid-test-file.pdf";

            var fakeAttachment = new TaskAttachment
            {
                Id = attachmentId,
                ProjectTaskId = taskId,
                StoredFileName = blobFileName
            };

            A.CallTo(() => _taskAttachmentRepository.GetAttachmentByIdAsync(attachmentId))
                .Returns(fakeAttachment);

            var command = new DeleteAttachmentCommand(projectId, taskId, userId, attachmentId);

            // Act

            await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _entityValidationService.EnsureAttachmentBelongsToTaskAsync(attachmentId, taskId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _blobStorage.DeleteFileAsync("task-attachments", blobFileName, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _taskAttachmentRepository.DeleteAttachmentByIdAsync(attachmentId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly());
        }
    }
}
