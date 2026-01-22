using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.IntegrationTests.Features.Projects
{
    public class GetAllProjectsByUserIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnPagedProjects_ForUser()
        {
            // --------------------
            // ARRANGE
            // --------------------

            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var repository = new ProjectRepository(context);
            var logger = NullLogger<GetAllProjectsByUserIdQueryHandler>.Instance;

            var handler = new GetAllProjectsByUserIdQueryHandler(logger, repository);

            var query = new GetAllProjectsByUserIdQuery(
                "user-123",
                new ProjectQueryParams
                {
                    PageNumber = 1,
                    PageSize = 10
                });

            // --------------------
            // ACT
            // --------------------

            var result = await handler.Handle(query, CancellationToken.None);

            // --------------------
            // ASSERT
            // --------------------

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(2);
            result.Items.Should().HaveCount(2);

            result.Items.Select(p => p.Name)
                .Should()
                .Contain(new[] { "Project 1", "Project 2" });

           
        }
    }   
}
