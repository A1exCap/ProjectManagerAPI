using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.Features.Tasks.Commands.MarkTaskStarted;
using ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery;
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
    public class GetTaskByIdQueryHandlerTests
    {
        private readonly IAccessService _accessService;
        private readonly ILogger<GetTaskByIdQueryHandler> _logger;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IEntityValidationService _entityValidationService;

        private readonly GetTaskByIdQueryHandler _handler;

        public GetTaskByIdQueryHandlerTests()
        {
            _logger = NullLogger<GetTaskByIdQueryHandler>.Instance;

            _projectTaskRepository = A.Fake<IProjectTaskRepository>();
            _accessService = A.Fake<IAccessService>();
            _entityValidationService = A.Fake<IEntityValidationService>();

            _handler = new GetTaskByIdQueryHandler(_projectTaskRepository, _logger, _accessService, _entityValidationService);
        }

        [Fact]
        public async Task GetTaskByIdQueryHandler_ShouldReturnTask()
        {
            // Arrange

            int taskId = 1;
            int projectId = 1;
            string userId = "user-123";

            var query = new GetTaskByIdQuery(taskId, projectId, userId);

            var task = new ProjectTask()
            {
                Id = taskId,
                ProjectId = projectId,
                Title = "Test Task Title",
                Status = ProjectTaskStatus.ToDo
            };

            A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId))
                .Returns(task);

            // Act

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId)).MustHaveHappenedOnceExactly());

            result.Should().NotBeNull();
            result.Title.Should().Be(task.Title);
            result.Status.Should().Be(task.Status);
        }
    }
}
