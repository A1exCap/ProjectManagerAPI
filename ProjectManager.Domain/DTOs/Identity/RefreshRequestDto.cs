using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.DTOs.Identity
{
    public class RefreshRequestDto
    {
        [Required]
        public string? RefreshToken { get; set; }
    }
}
