using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Commands.UpdateProjectCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManagerAPI.Tests.Features.Projects
{
    public class UpdateProjectCommandHandlerTests
    {
        private readonly ILogger<UpdateProjectCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;

        private readonly UpdateProjectCommandHandler _handler;

        public UpdateProjectCommandHandlerTests()
        {
            _logger = NullLogger<UpdateProjectCommandHandler>.Instance;
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _projectRepository = A.Fake<IProjectRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateProjectCommandHandler(
                _logger,
                _entityValidationService,
                _accessService,
                _projectRepository,
                _unitOfWork
            );
        }

        [Fact]
        public async Task Handle_ShouldUpdateProject()
        {
            // ARRANGE
            int projectId = 1;
            string userId = "user-123";

            var dto = new ProjectUpdateDto
            {
                Name = "Updated Project",
                Description = "Updated Description",
                EndDate = DateTime.UtcNow.AddDays(60),
                Status = ProjectStatus.Completed,
                Visibility = ProjectVisibility.Public,
                ClientName = "Updated Client",
                Budget = 2000,
                Technologies = "Java; Spring"
            };

            var command = new UpdateProjectCommand(projectId, userId, dto);

            var project = new Project
            {
                Name = "Old Name",
                Description = "Old Description"
            };

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .Returns(project);

            // ACT
            await _handler.Handle(command, CancellationToken.None);

            // ASSERT 
            project.Name.Should().Be(dto.Name);
            project.Description.Should().Be(dto.Description);
            project.EndDate.Should().Be(dto.EndDate);
            project.Status.Should().Be(dto.Status);
            project.Visibility.Should().Be(dto.Visibility);
            project.ClientName.Should().Be(dto.ClientName);
            project.Budget.Should().Be(dto.Budget);
            project.Technologies.Should().Be(dto.Technologies);

            // ASSERT —
            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _accessService.EnsureUserIsProjectOwnerAsync(projectId, userId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(projectId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _projectRepository.UpdateProject(project))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.SaveChangesAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
