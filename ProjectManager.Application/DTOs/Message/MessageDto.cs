using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.DTOs.Message
{
    public record MessageDto
    {
        [Required]
        public NotificationType NotificationType { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public RelatedEntityType EntityType { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
