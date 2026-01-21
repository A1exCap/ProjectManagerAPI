using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Features.Messages.Queries.GetAllMessagesByUserQuery;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.IntegrationTests.Features.Messages
{
    public class GetAllMessagesByUserQueryHandlerTests
    {
        private readonly ILogger<GetAllMessagesByUserQueryHandler> _logger;

        public GetAllMessagesByUserQueryHandlerTests()
        {
            _logger = NullLogger<GetAllMessagesByUserQueryHandler>.Instance;
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedMessages()
        {
            // Arrange

            using var context = await TestDbContextFactory.CreateWithDefaultValues();

            var user = context.Users.First(u => u.UserName == "TestUser1");

            context.Messages.AddRange(
                new Message
                {
                    User = user,
                    Title = "Oldest Message",
                    CreatedAt = DateTime.UtcNow.AddHours(-2) 
                },
                new Message
                {
                    User = user,
                    Title = "Middle Message",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new Message
                {
                    User = user,
                    Title = "Newest Message",
                    CreatedAt = DateTime.UtcNow 
                } 
            );

            await context.SaveChangesAsync();

            var repository = new MessageRepository(context);

            var handler = new GetAllMessagesByUserQueryHandler(_logger, repository);

            var query = new GetAllMessagesByUserQuery(user.Id, new MessageQueryParams
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

            result.Items.First().Title.Should().Be("Newest Message");
            result.Items.Last().Title.Should().Be("Middle Message");
        }
    }
}
