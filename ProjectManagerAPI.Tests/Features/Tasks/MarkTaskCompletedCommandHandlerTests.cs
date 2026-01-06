using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskCompleted;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Tasks
{
    public class MarkTaskCompletedCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<MarkTaskCompletedCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICreateMessageService _messageService;
        private readonly IAccessService _accessService;

        private readonly MarkTaskCompletedCommandHandler _handler;

        public MarkTaskCompletedCommandHandlerTests()
        {
            _logger = NullLogger<MarkTaskCompletedCommandHandler>.Instance;

            _unitOfWork = A.Fake<IUnitOfWork>();
            _projectTaskRepository = A.Fake<IProjectTaskRepository>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _messageService = A.Fake<ICreateMessageService>();
            _accessService = A.Fake<IAccessService>();

            _handler = new MarkTaskCompletedCommandHandler(_unitOfWork, _entityValidationService, _projectTaskRepository, _logger, _accessService, _messageService);
        }

        [Fact]
        public async Task Handle_ShouldMarkTaskAsCompleted()
        {
            // Arrange
            var projectId = 1;
            var taskId = 1;
            var userId = "user-123";

            var command = new MarkTaskCompletedCommand(projectId, taskId, userId);

            var existingTask = new ProjectTask
            {
                Id = taskId,
                Title = "Fix critical bug",
                ProjectId = projectId,
                Status = ProjectTaskStatus.ToDo,
                CreatorId = "creator-123"
            };

            A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId))
                .Returns(existingTask);

            // Act

            await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Contributor" }))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.UpdateTask(existingTask)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _messageService.CreateAsync(existingTask.CreatorId, NotificationType.TaskCompleted, $"Task '{existingTask.Title}' has been marked as completed.", RelatedEntityType.ProjectTask, existingTask.Id)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            existingTask.Status.Should().Be(ProjectTaskStatus.Done);
            existingTask.CompletedAt.Should().NotBeNull();
        }
    }
}
