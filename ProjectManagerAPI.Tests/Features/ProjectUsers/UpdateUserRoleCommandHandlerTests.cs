using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.ProjectUsers.Commands.UpdateUserRoleCommand;
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

namespace ProjectManagerAPI.Tests.Features.ProjectUsers
{
    public class UpdateUserRoleCommandHandlerTests
    {
        private readonly ILogger<UpdateUserRoleCommandHandler> _logger;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICreateMessageService _messageService;
        private readonly IAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;

        private readonly UpdateUserRoleCommandHandler _handler;

        public UpdateUserRoleCommandHandlerTests()
        {
            _logger = NullLogger<UpdateUserRoleCommandHandler>.Instance;

            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _projectUserRepository = A.Fake<IProjectUserRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _messageService = A.Fake<ICreateMessageService>();
            _projectRepository = A.Fake<IProjectRepository>();

            _handler = new UpdateUserRoleCommandHandler(
                _logger,
                _accessService,
                _entityValidationService,
                _projectUserRepository,
                _unitOfWork,
                _messageService,
                _projectRepository
            );
        }

        [Fact]
        public async Task Should_UpdateUserRole()
        {
            // Arrange
           
            int projectId = 1;
            string userId = "user-123";
            string userToUpdateId = "user-to-update-123";

            var newRoleString = "Manager";
            var newRoleEnum = ProjectUserRole.Manager;

            decimal hourlyRate = 8.0m;

            var command = new UpdateProjectUserCommand(projectId, userId, userToUpdateId, newRoleString, hourlyRate);

            var projectUser = new ProjectUser
            {
                ProjectId = projectId,
                UserId = userToUpdateId,
                Role = ProjectUserRole.Contributor,
                HourlyRate = 0
            };

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project"
            };

            A.CallTo(() => _projectUserRepository.GetProjectUserIdAsync(projectId, userToUpdateId))
                .Returns(projectUser);

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .Returns(project);

            A.CallTo(() => _entityValidationService.EnsureRoleIsValid(newRoleString))
                .Returns(newRoleEnum);

            // Act

            await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Manager", "Owner" })))
               .MustHaveHappenedOnceExactly()
               .Then(A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _entityValidationService.EnsureUserIsProjectMemberAsync(projectId, userToUpdateId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _projectUserRepository.GetProjectUserIdAsync(projectId, userToUpdateId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _entityValidationService.EnsureRoleIsValid(newRoleString)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _projectUserRepository.UpdateProjectUser(A<ProjectUser>.That.Matches(u =>
                    u.Role == newRoleEnum &&
                    u.HourlyRate == hourlyRate
                    ))).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _messageService.CreateAsync(
                   userToUpdateId, NotificationType.ProjectRoleChanged, $"Your role in project {project.Name} has been changed to {newRoleString}.", RelatedEntityType.Project, projectId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            projectUser.Role.Should().Be(newRoleEnum);
            projectUser.HourlyRate.Should().Be(hourlyRate);
        }
    }
}
