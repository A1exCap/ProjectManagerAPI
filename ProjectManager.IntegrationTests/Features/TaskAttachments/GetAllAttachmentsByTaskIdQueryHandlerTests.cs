using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.TaskAttachments.Queries.GetAllAttachmentsByTaskIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.IntegrationTests.Features.TaskAttachments
{
    public class GetAllAttachmentsByTaskIdQueryHandlerTests
    {
        private readonly ILogger<GetAllAttachmentsByTaskIdQueryHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;

        public GetAllAttachmentsByTaskIdQueryHandlerTests()
        {
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _logger = NullLogger<GetAllAttachmentsByTaskIdQueryHandler>.Instance;
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedAttachments()
        {
            // Arrange

            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var userId = "user-123";

            var targetProject = await context.Projects.FirstAsync(p => p.OwnerId == userId);
            var projectId = targetProject.Id;   

            var task = new ProjectTask
            {
                ProjectId = projectId,
                Title = "Task 1",
                Description = "Description 1",
                CreatedAt = DateTime.UtcNow.AddHours(-5),
                CreatorId = userId,
            };

            await context.ProjectTasks.AddAsync(task);

            var attachments = new List<TaskAttachment>
            {
                new TaskAttachment
                {
                    FileName = "Oldest",
                    StoredFileName = "att1",
                    FilePath = "/path/to/att1",
                    ContentType = "pdf",
                    UploadedAt = DateTime.UtcNow.AddHours(-2),
                    UploadedById = userId,
                    ProjectTask = task
                },
                new TaskAttachment
                {
                    FileName = "Middle",
                    StoredFileName = "att2",
                    FilePath = "/path/to/att2",
                    ContentType = "pdf",
                    UploadedAt = DateTime.UtcNow.AddHours(-1),
                    UploadedById = userId,
                    ProjectTask = task
                },
                new TaskAttachment
                {
                    FileName = "Newest",
                    StoredFileName = "att3",
                    FilePath = "/path/to/att3",
                    ContentType = "pdf",
                    UploadedAt = DateTime.UtcNow,
                    UploadedById = userId,
                    ProjectTask = task
                }
            };
            await context.TaskAttachments.AddRangeAsync(attachments);
            await context.SaveChangesAsync();

            var repository = new TaskAttachmentRepository(context);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .Returns(Task.CompletedTask);
            A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(task.Id, projectId))
                .Returns(Task.CompletedTask);
            A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId))
                .Returns(Task.CompletedTask);

            var handler = new GetAllAttachmentsByTaskIdQueryHandler(_logger, _accessService, _entityValidationService, repository);

            var query = new GetAllAttachmentsByTaskIdQuery(projectId, userId, task.Id, new AttachmentQueryParams
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

            result.Items.First().FileName.Should().Be("Newest");
            result.Items.Last().FileName.Should().Be("Middle");
        }
    }
}
