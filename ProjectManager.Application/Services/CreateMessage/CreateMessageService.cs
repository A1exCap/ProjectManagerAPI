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
        public CreateMessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task CreateAsync(string? userId, NotificationType type, string title, RelatedEntityType entityType, 
            int relatedEntityId)
        {
            var message = new Message
            {
                UserId = userId,
                NotificationType = type,
                Title = title,
                EntityType = entityType,
                RelatedEntityId = relatedEntityId,
            };

            await _messageRepository.AddMessageAsync(message);
        }
    }
}
