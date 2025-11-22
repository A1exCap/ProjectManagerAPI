using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Messages.Commands.DeleteMessagesByUserIdCommand;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Messages.Commands.DeleteMessageCommand
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Unit>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteMessageCommandHandler> _logger;
        public DeleteMessageCommandHandler(IMessageRepository messageRepository, ILogger<DeleteMessageCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteMessageCommandHandler by messageId: {MessageId}", request.MessageId);

            await _messageRepository.DeleteMessageByIdAsync(request.MessageId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Message with ID {MessageId} deleted successfully", request.MessageId);
            return Unit.Value;
        }
    }
}
