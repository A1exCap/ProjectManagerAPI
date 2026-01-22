using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted;
using ProjectManager.Application.Services.Access;
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
    public class MarkTaskStartedCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<MarkTaskStartedCommandHandler> _logger;
        private readonly IAccessService _accessService;
        private readonly IEntityValidationService _entityValidationService;

        private readonly MarkTaskStartedCommandHandler _handler;

        public MarkTaskStartedCommandHandlerTests()
        {
            _logger = NullLogger<MarkTaskStartedCommandHandler>.Instance;

            _unitOfWork = A.Fake<IUnitOfWork>();
            _projectTaskRepository = A.Fake<IProjectTaskRepository>();
            _accessService = A.Fake<IAccessService>();
            _entityValidationService = A.Fake<IEntityValidationService>();

            _handler = new MarkTaskStartedCommandHandler(_unitOfWork, _accessService, _projectTaskRepository, _logger, _entityValidationService);
        }

        [Fact]
        public async Task Handle_ShouldMarkTaskAsStarted()
        {
            // Arrange
            var projectId = 1;
            var taskId = 1;
            var userId = "user-123";

            var command = new MarkTaskStartedCommand(projectId, taskId, userId);

            var existingTask = new ProjectTask
            {
                Id = taskId,
                ProjectId = projectId,
                Status = ProjectTaskStatus.ToDo
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
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());
          
            existingTask.Status.Should().Be(ProjectTaskStatus.InProgress);
        }
    }
}
