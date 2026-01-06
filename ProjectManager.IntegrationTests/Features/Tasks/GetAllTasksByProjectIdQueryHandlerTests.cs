using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore; // Потрібен для FirstAsync
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using Xunit;

namespace ProjectManager.IntegrationTests.Features.Tasks
{
    public class GetAllTasksByProjectIdQueryHandlerTests
    {
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ILogger<GetAllTasksByProjectIdQueryHandler> _logger;

        public GetAllTasksByProjectIdQueryHandlerTests()
        {
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _logger = NullLogger<GetAllTasksByProjectIdQueryHandler>.Instance;
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredAndSortedTasks()
        {
            // ARRANGE
            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var userId = "user-123";

            var targetProject = await context.Projects.FirstAsync(p => p.Name == "Project 1");
            var targetProjectId = targetProject.Id;

            var otherProject = await context.Projects.FirstAsync(p => p.Name == "Project 3");
            var otherProjectId = otherProject.Id;

            context.ProjectTasks.AddRange(
                new ProjectTask
                {
                    ProjectId = targetProjectId, 
                    Title = "Alpha Task",
                    Priority = ProjectTaskPriority.Low, 
                    Status = ProjectTaskStatus.ToDo,
                    CreatorId = userId
                },
                new ProjectTask
                {
                    ProjectId = targetProjectId, 
                    Title = "Charlie Task",
                    Priority = ProjectTaskPriority.High, 
                    Status = ProjectTaskStatus.InProgress,
                    CreatorId = userId
                },
                new ProjectTask
                {
                    ProjectId = targetProjectId,
                    Title = "Bravo Task",
                    Priority = ProjectTaskPriority.High, 
                    Status = ProjectTaskStatus.Done,
                    CreatorId = userId
                },
                new ProjectTask
                {
                    ProjectId = otherProjectId, 
                    Title = "Other Project Task",
                    Priority = ProjectTaskPriority.High,
                    Status = ProjectTaskStatus.ToDo,
                    CreatorId = userId
                }
            );

            await context.SaveChangesAsync();

            var repository = new ProjectTaskRepository(context);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(targetProjectId))
                .Returns(Task.CompletedTask);
            A.CallTo(() => _accessService.EnsureUserHasAccessAsync(targetProjectId, userId))
                .Returns(Task.CompletedTask);

            var queryParams = new TaskQueryParams
            {
                PageNumber = 1,
                PageSize = 10,
                Priority = ProjectTaskPriority.High,
                SortBy = "title",
                SortDescending = true
            };

            var query = new GetAllTasksByProjectIdQuery(targetProjectId, userId, queryParams);

            var handler = new GetAllTasksByProjectIdQueryHandler(
                repository,
                _logger,
                _entityValidationService,
                _accessService
            );

            // ACT

            var result = await handler.Handle(query, CancellationToken.None);

            // ASSERT

            result.TotalCount.Should().Be(2);

            result.Items.First().Title.Should().Be("Charlie Task");
            result.Items.Last().Title.Should().Be("Bravo Task");

            result.Items.All(t => t.Priority == ProjectTaskPriority.High).Should().BeTrue();
        }
    }
}