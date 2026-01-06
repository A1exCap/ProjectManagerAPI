using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Features.Tasks.Commands.UpdateTask;
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
    public class UpdateTaskCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;

        private readonly UpdateTaskCommandHandler _handler;

        public UpdateTaskCommandHandlerTests()
        {
            _logger = NullLogger<UpdateTaskCommandHandler>.Instance;
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userManager = A.Fake<UserManager<User>>();
            _projectTaskRepository = A.Fake<IProjectTaskRepository>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();

            _handler = new UpdateTaskCommandHandler(_logger, _userManager, _projectTaskRepository, _unitOfWork, _entityValidationService, _accessService);
        }

        [Fact]
        public async Task Handle_ShouldUpdateTask()
        {
            // Arrange

            var projectId = 1;
            var taskId = 1;
            var userId = "user-123";

            var updateDto = new ProjectTaskUpdateDto
            {
                Title = "Updated Task Title",
                Description = "Updated Description",
                AssigneeEmail = "new-user@test.com"
            };

            var command = new UpdateTaskCommand(projectId, taskId, userId, updateDto);

            var existingTask = new ProjectTask
            {
                Id = taskId,
                ProjectId = projectId,
                Title = "Old Task Title",
                Description = "Old Description"
            };

            var user = new User
            {
                Id = "assignee-123",
                Email = "new-user@test.com"
            };

            A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId))
                .Returns(existingTask);

            A.CallTo(() => _userManager.FindByEmailAsync("new-user@test.com"))
                .Returns(user);

            // Act

            await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Owner", "Manager" }))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _userManager.FindByEmailAsync(updateDto.AssigneeEmail)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.UpdateTask(existingTask)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            existingTask.Title.Should().Be(updateDto.Title);
            existingTask.Description.Should().Be(updateDto.Description);
            existingTask.AssigneeId.Should().Be(user.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAssigneeEmailDoesNotExist()
        {

            // ARRANGE

            var projectId = 1;
            var taskId = 1;
            var userId = "user-123";
            var nonExistentEmail = "ghost@gmail.com";

            var updateDto = new ProjectTaskUpdateDto
            {
                Title = "New Title",
                AssigneeEmail = nonExistentEmail 
            };

            var command = new UpdateTaskCommand(projectId, taskId, userId, updateDto);

            A.CallTo(() => _userManager.FindByEmailAsync(nonExistentEmail))
                .Returns((User?)null);

            // ACT

            await _handler.Awaiting(h => h.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"User with email '{nonExistentEmail}' not found.");

            // ASSERT 

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _userManager.FindByEmailAsync(nonExistentEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _projectTaskRepository.UpdateTask(A<ProjectTask>._))
                .MustNotHaveHappened(); 
            A.CallTo(() => _unitOfWork.SaveChangesAsync(A<CancellationToken>._))
                .MustNotHaveHappened();
        }
    }
}
