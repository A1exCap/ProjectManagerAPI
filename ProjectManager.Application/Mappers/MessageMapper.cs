using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Message;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Mappers
{
    public static class MessageMapper
    {
        public static MessageDto ToDto(Message message)
        {
            return new MessageDto
            {
                NotificationType = message.NotificationType,
                Title = message.Title,
                EntityType = message.EntityType,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
