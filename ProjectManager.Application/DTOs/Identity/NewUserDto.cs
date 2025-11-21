using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.DTOs.Account
{
    public record NewUserDto
    {
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
