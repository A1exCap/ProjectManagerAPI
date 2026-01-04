using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Queries.GetProjectDetailsByIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Projects
{
    public class GetProjectDetailsQueryHandlerTests
    {
        private readonly ILogger<GetProjectDetailsByIdQueryHandler> _logger;
        private readonly IAccessService _accessService;
        private readonly IProjectRepository _projectRepository;
        private readonly IEntityValidationService _entityValidationService;

        private readonly GetProjectDetailsByIdQueryHandler _handler;

        public GetProjectDetailsQueryHandlerTests()
        {
            _logger = NullLogger<GetProjectDetailsByIdQueryHandler>.Instance;

            _accessService = A.Fake<IAccessService>();
            _projectRepository = A.Fake<IProjectRepository>();
            _entityValidationService = A.Fake<IEntityValidationService>();

            _handler = new GetProjectDetailsByIdQueryHandler(
                _logger,
                _projectRepository,
                _entityValidationService,
                _accessService
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectDetails()
        {
            // Arrange
            var query = new GetProjectDetailsByIdQuery(1, "user-123");
            var fakeProject = new Project { Id = 1, Name = "Test Project" };

            A.CallTo(() => _projectRepository.GetByProjectIdAsync(1))
                .Returns(fakeProject);

            // Act

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert

            // Assert
            // 1. Перевіряємо дані (це автоматично перевіряє і тип, і роботу маппера)
            result.Should().BeEquivalentTo(new { Name = "Test Project" });

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(1))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _accessService.EnsureUserHasAccessAsync(1, "user-123")).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectRepository.GetByProjectIdAsync(1)).MustHaveHappenedOnceExactly());
        }
    }
}
