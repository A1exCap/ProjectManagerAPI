using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;

namespace ProjectManager.IntegrationTests.Features.Documents
{
    public class GetAllDocumentsByProjectIdQueryHandlerTests
    {
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ILogger<GetAllDocumentsByProjectIdQueryHandler> _logger;

        public GetAllDocumentsByProjectIdQueryHandlerTests()
        {
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _logger = NullLogger<GetAllDocumentsByProjectIdQueryHandler>.Instance;
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedDocuments_OrderedByDate()
        {
            // --------------------
            // ARRANGE
            // --------------------

            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var userId = "user-123"; 

            var targetProject = await context.Projects.FirstAsync(p => p.OwnerId == userId);
            var projectId = targetProject.Id;

            var otherProject = await context.Projects.FirstAsync(p => p.OwnerId != userId);
            var otherProjectId = otherProject.Id;

            context.ProjectDocuments.AddRange(
                new ProjectDocument
                {
                    ProjectId = projectId,
                    Name = "Oldest Doc",
                    UploadedAt = DateTime.UtcNow.AddHours(-5),
                    StoredFileName = "f1",
                    FilePath = "p1",
                    ContentType = "pdf",
                    UploadedById = userId
                },
                new ProjectDocument
                {
                    ProjectId = projectId,
                    Name = "Middle Doc",
                    UploadedAt = DateTime.UtcNow.AddHours(-3),
                    StoredFileName = "f2",
                    FilePath = "p2",
                    ContentType = "pdf",
                    UploadedById = userId
                },
                new ProjectDocument
                {
                    ProjectId = projectId,
                    Name = "Newest Doc",
                    UploadedAt = DateTime.UtcNow.AddHours(-1),
                    StoredFileName = "f3",
                    FilePath = "p3",
                    ContentType = "pdf",
                    UploadedById = userId
                },
                new ProjectDocument
                {
                    ProjectId = otherProjectId, 
                    Name = "Other Project Doc",
                    UploadedAt = DateTime.UtcNow,
                    StoredFileName = "f4",
                    FilePath = "p4",
                    ContentType = "pdf",
                    UploadedById = userId
                }
            );

            await context.SaveChangesAsync();

            var repository = new ProjectDocumentRepository(context);

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .Returns(Task.CompletedTask);
            A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId))
                .Returns(Task.CompletedTask);

            var handler = new GetAllDocumentsByProjectIdQueryHandler(
                _logger,
                _accessService,
                _entityValidationService,
                repository
            );

            var query = new GetAllDocumentsByProjectIdQuery(
                projectId,
                userId,
                new DocumentQueryParams { PageNumber = 1, PageSize = 2 }
            );

            // --------------------
            // ACT
            // --------------------

            var result = await handler.Handle(query, CancellationToken.None);

            // --------------------
            // ASSERT
            // --------------------

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(3); // 3 документи в цьому проекті
            result.Items.Should().HaveCount(2); // 2 документи на сторінці

            result.Items.First().Name.Should().Be("Newest Doc");
            result.Items.Last().Name.Should().Be("Middle Doc");
        }
    }
}
