using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.ProjectUsers.Commands.CreateProjectUserCommand;
using ProjectManager.Application.Features.ProjectUsers.Commands.DeleteUserFromProjectCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.ProjectUsers
{
    public class DeleteProjectUserCommandHandlerTests
    {
        private readonly ILogger<DeleteProjectUserCommandHandler> _logger;
        private readonly IAccessService _accessService;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly DeleteProjectUserCommandHandler _handler;

        public DeleteProjectUserCommandHandlerTests()
        {
            _logger = NullLogger<DeleteProjectUserCommandHandler>.Instance;

            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _projectUserRepository = A.Fake<IProjectUserRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new DeleteProjectUserCommandHandler(
                _logger,
                _accessService,
                _entityValidationService,
                _projectUserRepository,
                _unitOfWork
            );
        }

        [Fact]
        public async Task Should_DeleteProjectUser()
        {
            // Arrange

            int projectId = 1;
            string userToDeleteId = "user-to-delete-123";
            string userId = "user-123";

            var projectUser = new ProjectUser
            {
                ProjectId = projectId,
                UserId = userToDeleteId
            };

            A.CallTo(() => _projectUserRepository.GetProjectUserIdAsync(projectId, userToDeleteId))
                .Returns(projectUser);

            var command = new DeleteProjectUserCommand(projectId, userToDeleteId, userId);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
               .MustHaveHappenedOnceExactly()
               .Then(A.CallTo(() => _entityValidationService.EnsureUserIsProjectMemberAsync(projectId, userToDeleteId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _accessService.EnsureUserHasRoleAsync(projectId, userId, A<string[]>.That.IsSameSequenceAs(new[] { "Manager", "Owner" }))).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _projectUserRepository.GetProjectUserIdAsync(projectId, userToDeleteId)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _projectUserRepository.DeleteProjectUser(projectUser)).MustHaveHappenedOnceExactly())
               .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());
        }
    }
}
