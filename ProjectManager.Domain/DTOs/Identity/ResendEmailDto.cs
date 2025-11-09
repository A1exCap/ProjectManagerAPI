using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.DTOs.Identity
{
    public class ResendEmailDto
    {
        public string? Email { get; set; }
        public string? ClientUrl { get; set; }
    }
}
