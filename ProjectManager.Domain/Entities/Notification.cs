﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; } 
        public User User { get; set; } = null!;
        public NotificationType NotificationType { get; set; }  
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public RelatedEntityType EntityType { get; set; }
        public int RelatedEntityId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum NotificationType
    {
        TaskAssigned,
        TaskCompleted,
        MentionedInComment,
        ProjectInvite,
        DeadlineReminder,
        NewComment,
        ProjectRoleChanged,
    }
    public enum RelatedEntityType
    {
        Project,
        ProjectTask,
        Comment,
    }
}
