using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Commands.DeleteTask;
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
    public class DeleteTaskCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;

        private readonly DeleteTaskCommandHandler _handler;

        public DeleteTaskCommandHandlerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _projectTaskRepository = A.Fake<IProjectTaskRepository>();
            _logger = NullLogger<DeleteTaskCommandHandler>.Instance;
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();

            _handler = new DeleteTaskCommandHandler(_unitOfWork, _entityValidationService, _projectTaskRepository, _logger, _accessService);
        }
        [Fact]
        public async Task Handle_ShouldDeleteTask()
        {
            // Arrange

            var projectId = 1;
            var userId = "user-123";
            var taskId = 1;

            var command = new DeleteTaskCommand(projectId, taskId, userId);

            A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId)).Returns(new ProjectTask
            {
                Id = taskId,
                ProjectId = projectId
            });

            // Act

            await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Owner", "Manager" }))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.DeleteTaskByIdAsync(taskId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
