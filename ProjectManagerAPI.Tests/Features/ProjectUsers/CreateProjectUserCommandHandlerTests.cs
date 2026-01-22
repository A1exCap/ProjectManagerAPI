using Azure.Storage.Blobs.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.ProjectUser;
using ProjectManager.Application.Features.Comments.Commands.CreateCommentCommand;
using ProjectManager.Application.Features.ProjectUsers.Commands.CreateProjectUserCommand;
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
    public class CreateProjectUserCommandHandlerTests
    {
        private readonly ILogger<CreateProjectUserCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ICreateMessageService _messageService;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly CreateProjectUserCommandHandler _handler;

        public CreateProjectUserCommandHandlerTests()
        {
            _logger = NullLogger<CreateProjectUserCommandHandler>.Instance;

            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _messageService = A.Fake<ICreateMessageService>();
            _projectRepository = A.Fake<IProjectRepository>();
            _projectUserRepository = A.Fake<IProjectUserRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new CreateProjectUserCommandHandler(
                _logger,
                _entityValidationService,
                _accessService,
                _projectUserRepository,
                _unitOfWork,
                _messageService,
                _projectRepository
            );
        }

        [Fact]
        public async Task Should_CreateProjectUser() {

            // Arrange

            int projectId = 1;
            string userId = "user-123";

            var dto = new AddUserToProjectDto
            {
                UserToAddId = "new-user-456",
                UserRole = "Contributor"
            };

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project"
            };

            var expectedRoleEnum = ProjectUserRole.Contributor;

            A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Manager", "Owner" })))
                .Returns(true);
            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .Returns(project);
            A.CallTo(() => _entityValidationService.EnsureRoleIsValid(dto.UserRole))
                .Returns(expectedRoleEnum);
            A.CallTo(() => _projectUserRepository.AddProjectUserAsync(A<ProjectUser>._))
                .Invokes((ProjectUser pu) => pu.Id = 123);

            var command = new CreateProjectUserCommand(projectId, userId, dto);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Manager", "Owner" }))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _entityValidationService.EnsureRoleIsValid(dto.UserRole)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectUserRepository.AddProjectUserAsync(A<ProjectUser>.That.Matches(
                 pu => pu.ProjectId == projectId && pu.UserId == dto.UserToAddId && pu.Role == expectedRoleEnum))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _messageService.CreateAsync(
                 dto.UserToAddId, NotificationType.ProjectInvite, $"You have been added to project {project.Name} with role {dto.UserRole}.", RelatedEntityType.Project, projectId)
                ).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            result.Should().Be(123);
        }
    }
}
