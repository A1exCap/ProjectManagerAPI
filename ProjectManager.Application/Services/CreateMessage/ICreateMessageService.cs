using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.CreateMessage
{
    public interface ICreateMessageService
    {
        Task CreateAsync(string? userId, NotificationType type, string title,
            RelatedEntityType entityType, int relatedEntityId);
    }
}
