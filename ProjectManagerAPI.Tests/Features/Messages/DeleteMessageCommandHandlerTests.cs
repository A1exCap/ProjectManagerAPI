using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Comments.Commands.UpdateCommentCommand;
using ProjectManager.Application.Features.Messages.Commands.DeleteMessageCommand;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Messages
{
    public class DeleteMessageCommandHandlerTests
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteMessageCommandHandler> _logger;

        private readonly DeleteMessageCommandHandler _handler;

        public DeleteMessageCommandHandlerTests()
        {
            _messageRepository = A.Fake<IMessageRepository>();  
            _unitOfWork = A.Fake<IUnitOfWork>();

            _logger = NullLogger<DeleteMessageCommandHandler>.Instance;
            _handler = new DeleteMessageCommandHandler(_messageRepository, _logger, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ShouldDeleteMessage()
        {
            // Arrange

            var userId = "user-123";
            int messageId = 1;

            var command = new DeleteMessageCommand(userId, messageId);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _messageRepository.DeleteMessageByIdAsync(messageId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            result.Should().Be(Unit.Value);
        }
    }
}
