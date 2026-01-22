using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.Comments.Queries.GetAllCommentsByTaskIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.IntegrationTests.Features.Comments
{
    public class GetAllCommentsByTaskIdQueryHandlerTests
    {
        private readonly ILogger<GetAllCommentsByTaskIdQueryHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;

        public GetAllCommentsByTaskIdQueryHandlerTests()
        {
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _logger = NullLogger<GetAllCommentsByTaskIdQueryHandler>.Instance;
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedComments()
        {
            // Arrange

            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var project1 = await context.Projects.FirstAsync(p => p.Name == "Project 1");
            var project2 = await context.Projects.FirstAsync(p => p.Name == "Project 2");

            var task1 = new ProjectTask
            {
                Title = "Task 1",
                Description = "Description 1",
                CreatorId = "user-123",
                ProjectId = project1.Id 
            };

            var task2 = new ProjectTask
            {
                Title = "Task 2",
                Description = "Description 2",
                CreatorId = "another-user",
                ProjectId = project2.Id
            };

            context.ProjectTasks.AddRange(task1, task2);

            context.Comments.AddRange(
                new Comment
                {
                    Content = "Comment 1 on Task 1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    ProjectTask = task1,
                    AuthorId = "user-123"
                },
                new Comment
                {
                    Content = "Comment 2 on Task 1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    ProjectTask = task1,
                    AuthorId = "another-user"
                },
                new Comment
                {
                    Content = "Comment 3 on Task 1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                    ProjectTask = task1,
                    AuthorId = "another-user"
                },
                new Comment
                {
                    Content = "Comment on Task 2",
                    CreatedAt = DateTime.UtcNow,
                    ProjectTask = task2,
                    AuthorId = "another-user"
                }
            );

            await context.SaveChangesAsync();

            var repository = new CommentRepository(context);

            var userId = "user-123";
            var projectId = project1.Id;
            var taskId = task1.Id;

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .Returns(Task.CompletedTask);
            A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId))
                .Returns(Task.CompletedTask);
            A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId))
                .Returns(Task.CompletedTask);

            var handler = new GetAllCommentsByTaskIdQueryHandler(_logger, repository, _entityValidationService, _accessService);

            var query = new GetAllCommentsByTaskIdQuery(projectId, userId, taskId, new CommentQueryParams
            {
                PageNumber = 1,
                PageSize = 2
            }); 

            // Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(3); 
            result.Items.Should().HaveCount(2); 

            result.Items.First().Content.Should().Be("Comment 1 on Task 1");
            result.Items.Last().Content.Should().Be("Comment 2 on Task 1");
        }
    }
}
