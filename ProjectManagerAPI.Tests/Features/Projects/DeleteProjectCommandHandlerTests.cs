using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Projects.Commands.CreateProjectCommand;
using ProjectManager.Application.Features.Projects.Commands.DeleteProjectCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Projects
{
    public class DeleteProjectCommandHandlerTests
    {
        private readonly ILogger<DeleteProjectCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;

        private readonly DeleteProjectCommandHandler _handler;

        public DeleteProjectCommandHandlerTests()
        {
            _logger = NullLogger<DeleteProjectCommandHandler>.Instance;

            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _projectRepository = A.Fake<IProjectRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new DeleteProjectCommandHandler(
                _logger,
                _entityValidationService,
                _accessService,
                _unitOfWork,
                _projectRepository
            );
        }

        [Fact]
        public async Task Handle_ShouldDeleteProject()
        {
            // Arrange
            var projectId = 1;
            var userId = "user-123";
            var command = new DeleteProjectCommand(projectId, userId);
            // Act
            await _handler.Handle(command, CancellationToken.None);
            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
              .MustHaveHappenedOnceExactly()
              .Then(A.CallTo(() => _accessService.EnsureUserIsProjectOwnerAsync(projectId, userId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _projectRepository.DeleteProjectAsync(projectId)).MustHaveHappenedOnceExactly())
              .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());     
        }
    }
}
