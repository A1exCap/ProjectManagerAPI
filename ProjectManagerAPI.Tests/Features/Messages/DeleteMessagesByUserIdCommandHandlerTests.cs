using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Messages.Commands.DeleteMessageCommand;
using ProjectManager.Application.Features.Messages.Commands.DeleteMessagesByUserIdCommand;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Messages
{
    public class DeleteMessagesByUserIdCommandHandlerTests
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<DeleteMessagesByUserIdCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private readonly DeleteMessagesByUserIdCommandHandler _handler;

        public DeleteMessagesByUserIdCommandHandlerTests()
        {
            _messageRepository = A.Fake<IMessageRepository>();  
            _unitOfWork = A.Fake<IUnitOfWork>();
            _logger = NullLogger<DeleteMessagesByUserIdCommandHandler>.Instance;

            _handler = new DeleteMessagesByUserIdCommandHandler(_messageRepository, _logger, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAllMessages()
        {
            // Arrange

            var userId = "user-123";

            var command = new DeleteMessagesByUserIdCommand(userId);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _messageRepository.DeleteMessagesByUserId(userId))
              .MustHaveHappenedOnceExactly()
              .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            result.Should().Be(Unit.Value);
        }
    }
}
