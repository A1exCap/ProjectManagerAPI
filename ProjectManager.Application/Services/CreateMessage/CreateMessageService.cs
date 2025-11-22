using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.CreateMessage
{
    public class CreateMessageService : ICreateMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<CreateMessageService> _logger;
        public CreateMessageService(IMessageRepository messageRepository, ILogger<CreateMessageService> logger)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        public async Task CreateAsync(string? userId, NotificationType type, string title, RelatedEntityType entityType, 
            int relatedEntityId)
        {
            _logger.LogInformation("Creating message for userId: {UserId}, type: {Type}, title: {Title}", userId, type, title);

            var message = new Message
            {
                UserId = userId,
                NotificationType = type,
                Title = title,
                EntityType = entityType,
                RelatedEntityId = relatedEntityId,
            };

            await _messageRepository.AddMessageAsync(message);

            _logger.LogInformation("Message created with ID: {MessageId}", message.Id);
        }
    }
}
