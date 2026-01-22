using Castle.Core.Logging;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services.Access;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Services
{
    public class AccessServiceTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<AccessService> _logger;

        private readonly AccessService _accessService;
        public AccessServiceTests()
        {
            _projectRepository = A.Fake<IProjectRepository>();
            _projectUserRepository = A.Fake<IProjectUserRepository>();
            _commentRepository = A.Fake<ICommentRepository>();
            _logger = NullLogger<AccessService>.Instance;

            _accessService = new AccessService(_projectUserRepository, _logger, _commentRepository, _projectRepository);
        }

        //  EnsureUserIsCommentAuthorAsync ---

        [Fact]
        public async Task EnsureUserIsCommentAuthor_ShouldSucceed_WhenUserIsAuthor()
        {
            // Arrange
            var comment = new Comment { Id = 1, AuthorId = "my-id" };
            A.CallTo(() => _commentRepository.GetByIdAsync(1)).Returns(comment);

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserIsCommentAuthorAsync("my-id", 1);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task EnsureUserIsCommentAuthor_ShouldThrowNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            int commentId = 1;
            A.CallTo(() => _commentRepository.GetByIdAsync(commentId)).Returns((Comment?)null);

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserIsCommentAuthorAsync("user1", commentId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Comment with ID {commentId} does not exist.");
        }

        [Fact]
        public async Task EnsureUserIsCommentAuthor_ShouldThrowForbidden_WhenUserIsNotAuthor()
        {
            // Arrange
            var comment = new Comment { Id = 1, AuthorId = "owner-id" };
            A.CallTo(() => _commentRepository.GetByIdAsync(1)).Returns(comment);

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserIsCommentAuthorAsync("hacker-id", 1);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        //  EnsureUserHasAccessAsync ---

        [Fact]
        public async Task EnsureUserHasAccess_ShouldSucceed_WhenProjectIsPublic()
        {
            // Arrange
            var project = new Project { Id = 1, Visibility = ProjectVisibility.Public };
            A.CallTo(() => _projectRepository.GetByProjectIdAsync(1)).Returns(project);

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserHasAccessAsync(1, "user1");

            // Assert
            await act.Should().NotThrowAsync();
            A.CallTo(() => _projectUserRepository.ExistsAsync(A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task EnsureUserHasAccess_ShouldThrowForbidden_WhenProjectPrivateAndUserNotMember()
        {
            // Arrange
            var project = new Project { Id = 1, Visibility = ProjectVisibility.Private };
            A.CallTo(() => _projectRepository.GetByProjectIdAsync(1)).Returns(project);
            A.CallTo(() => _projectUserRepository.ExistsAsync(1, "user1")).Returns(false);

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserHasAccessAsync(1, "user1");

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        //  EnsureUserHasRoleAsync ---

        [Theory]
        [InlineData("Admin", new[] { "Admin" })]
        [InlineData("ADMIN", new[] { "admin" })]
        [InlineData("Manager", new[] { "Admin", "Manager", "Owner" })]
        public async Task EnsureUserHasRole_ShouldSucceed_WhenRoleMatches(string userDbRole, string[] allowedRoles)
        {
            // Arrange
            int projectId = 1;
            string userId = "u1";

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .Returns(new Project { Visibility = ProjectVisibility.Public });

            A.CallTo(() => _projectUserRepository.GetUserRoleAsync(projectId, userId))
                .Returns(userDbRole);

            // Act
            var result = await _accessService.EnsureUserHasRoleAsync(projectId, userId, allowedRoles);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task EnsureUserHasRole_ShouldThrowForbidden_WhenRoleDoesNotMatch()
        {
            // Arrange
            int projectId = 1;
            string userId = "u1";

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .Returns(new Project { Visibility = ProjectVisibility.Public });

            A.CallTo(() => _projectUserRepository.GetUserRoleAsync(projectId, userId)).Returns("Viewer");

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserHasRoleAsync(projectId, userId, new[] { "Admin" });

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task EnsureUserHasRole_ShouldThrowForbidden_WhenUserHasNoRole()
        {
            // Arrange
            int projectId = 1;

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .Returns(new Project { Visibility = ProjectVisibility.Public });

            A.CallTo(() => _projectUserRepository.GetUserRoleAsync(projectId, A<string>._)).
                Returns((string?)null);

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserHasRoleAsync(projectId, "u1", new[] { "Admin" });

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        // EnsureUserIsProjectOwnerAsync ---

        [Fact]
        public async Task EnsureUserIsProjectOwnerAsynce_ShouldThrowForbidden_WhenUserNotOwner()
        {
            // Arrange
            int projectId = 1;
            string userId = "not-owner";
            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
               .Returns(new Project { OwnerId = "owner" });

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserIsProjectOwnerAsync(projectId, userId);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task EnsureUserIsProjectOwnerAsynce_ShouldSucceed_WhenUserOwner()
        {
            // Arrange
            int projectId = 1;
            string userId = "owner";
            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
               .Returns(new Project { OwnerId = "owner" });

            // Act
            Func<Task> act = async () => await _accessService.EnsureUserIsProjectOwnerAsync(projectId, userId);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
