using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using ProjectManager.Application.Features.Projects.Queries.GetAllPublicProjectsByNameQuery;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.IntegrationTests.Features.Projects
{
    public class GetAllPublicProjectsByNameQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnPagedProjects_ByName()
        {
            // Arrange

            using var context = TestDbContextFactory.Create();

            context.Projects.AddRange(
                new Project { Name = "Public Project Alpha", Visibility = ProjectVisibility.Public, Status = ProjectStatus.Active },
                new Project { Name = "Public Project Beta", Visibility = ProjectVisibility.Public, Status = ProjectStatus.Completed },
                new Project { Name = "Private Project Gamma", Visibility = ProjectVisibility.Private, Status = ProjectStatus.Active }
            );

            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            var logger = NullLogger<GetAllPublicProjectsByNameQueryHandler>.Instance;

            var handler = new GetAllPublicProjectsByNameQueryHandler(logger, repository);   

            var query = new GetAllPublicProjectsByNameQuery(
                "Project",
                new ProjectQueryParams
                {
                    PageNumber = 1,
                    PageSize = 10
                });

            // Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(2);
            result.Items.Should().OnlyContain(p => p.Visibility == ProjectVisibility.Public);

            var items = result.Items.ToList();
            items[0].Name.Should().Be("Public Project Alpha"); 
            items[1].Name.Should().Be("Public Project Beta");  

            result.Items.Should().NotContain(p => p.Name.Contains("Gamma"));
        }
    }
}
