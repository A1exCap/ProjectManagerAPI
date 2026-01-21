using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Features.Projects.Commands.CreateProjectCommand;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
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
    public class CreateTaskCommandHandlerTests
    {
        private readonly ILogger<CreateTaskCommandHandler> _logger;
        private readonly IAccessService _accessService;
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICreateMessageService _messageService;
        private readonly IEntityValidationService _entityValidationService;
        private readonly UserManager<User> _userManager;

        private readonly CreateTaskCommandHandler _handler;

        public CreateTaskCommandHandlerTests()
        {
            _logger = NullLogger<CreateTaskCommandHandler>.Instance;
            
            _accessService = A.Fake<IAccessService>();
            _taskRepository = A.Fake<IProjectTaskRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _messageService = A.Fake<ICreateMessageService>();
            _entityValidationService = A.Fake<IEntityValidationService>();
            _userManager = A.Fake<UserManager<User>>();

            _handler = new CreateTaskCommandHandler(_taskRepository, _unitOfWork, _userManager, _accessService, _logger, _entityValidationService, _messageService);
        }

        [Fact]
        public async Task Handle_ShouldCreateTask()
        {
            // Arange

            int projectId = 1;
            string userId = "user-123";
            var dto = new CreateProjectTaskDto
            {
                Title = "New Task",
                Description = "Task Description",
                Priority = ProjectTaskPriority.Medium,
                AssigneeEmail = "assignee@gmail.com"
            };

            var assigneeUser = new User
            {
                Id = "assignee-123",
                Email = dto.AssigneeEmail
            };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.AssigneeEmail))
                .Returns(assigneeUser);

            A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Owner", "Manager" })))
                .Returns(true);

            A.CallTo(() => _taskRepository.AddTaskAsync(A<ProjectTask>._))
                .Invokes((ProjectTask t) => t.Id = 42) 
                .Returns(Task.CompletedTask);

            var command = new CreateTaskCommand(projectId, userId, dto);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Owner", "Manager" }))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _userManager.FindByEmailAsync(dto.AssigneeEmail)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _taskRepository.AddTaskAsync(
                    A<ProjectTask>.That.Matches(pt =>
                     pt.Title == dto.Title &&           
                     pt.ProjectId == projectId &&           
                     pt.AssigneeId == assigneeUser.Id &&   
                     pt.Priority == dto.Priority)
                    )).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _messageService.CreateAsync(assigneeUser.Id, NotificationType.TaskAssigned, $"You have been assigned to task: {dto.Title}", RelatedEntityType.ProjectTask, 42)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            result.Should().Be(42);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAssigneeEmailDoesNotExist()
        {
            // --------------------
            // ARRANGE
            // --------------------
            int projectId = 1;
            string userId = "user-123";
            string nonExistentEmail = "ghost@gmail.com";

            var dto = new CreateProjectTaskDto
            {
                Title = "New Task",
                AssigneeEmail = nonExistentEmail 
            };

            var command = new CreateTaskCommand(projectId, userId, dto);

            A.CallTo(() => _userManager.FindByEmailAsync(nonExistentEmail))
                .Returns((User?)null);

            // --------------------
            // ACT 
            // --------------------

            await _handler.Awaiting(h => h.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>() 
                .WithMessage($"User with email '{nonExistentEmail}' not found.");

            // --------------------
            // ASSERT SIDE EFFECTS 
            // --------------------

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _taskRepository.AddTaskAsync(A<ProjectTask>._))
                .MustNotHaveHappened();
            A.CallTo(() => _messageService.CreateAsync(A<string>._, A<NotificationType>._, A<string>._, A<RelatedEntityType>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None))
                .MustNotHaveHappened();
        }

    }
}
