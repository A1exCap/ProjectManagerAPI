using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Messages.Commands.DeleteMessagesByUserIdCommand
{
    public class DeleteMessagesByUserIdCommandHandler : IRequestHandler<DeleteMessagesByUserIdCommand, Unit>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<DeleteMessagesByUserIdCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteMessagesByUserIdCommandHandler(IMessageRepository messageRepository, ILogger<DeleteMessagesByUserIdCommandHandler> logger,
            IUnitOfWork unitOfWork) 
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteMessagesByUserIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteMessagesByUserIdCommandHandler by userId: {UserId}", request.UserId);

            _messageRepository.DeleteMessagesByUserId(request.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Messages for user with ID {UserId} deleted successfully", request.UserId);

            return Unit.Value;
        }
    }
}
