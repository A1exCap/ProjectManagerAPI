using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Application.Features.ProjectUsers.Queries.GetAllUsersByProjectIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.IntegrationTests.Features.ProjectUsers
{
    public class GetAllUsersByProjectIdQueryHandlerTests
    {

        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ILogger<GetAllUsersByProjectIdQueryHandler> _logger;

        public GetAllUsersByProjectIdQueryHandlerTests()
        {
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _logger = NullLogger<GetAllUsersByProjectIdQueryHandler>.Instance;
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedUsers_ForProject()
        {
            // Arrange

            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var repository = new ProjectUserRepository(context);

            var handler = new GetAllUsersByProjectIdQueryHandler(_logger, _accessService, _entityValidationService, repository);

            var targetProject = await context.Projects.FirstAsync(p => p.Name == "Project 1");
            var projectId = targetProject.Id;

            string userId = "user-123";

            var query = new GetAllUsersByProjectIdQuery(projectId, userId, new UsersQueryParams()
            {
                PageNumber = 1,
                PageSize = 10
            });

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
               .Returns(Task.CompletedTask);
            A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId))
                .Returns(Task.CompletedTask);

            // Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert   

            result.TotalCount.Should().Be(2);

            result.Items.Should().Contain(u => u.UserName == "TestUser1");
            result.Items.Should().Contain(u => u.UserName == "TestUser2");
        }
    }
}
