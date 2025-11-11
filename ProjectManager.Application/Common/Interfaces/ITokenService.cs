using ProjectManager.Domain.DTOs.Identity;
using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Abstractions.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDto> CreateToken(User user);
        string GenerateRefreshToken();
    }
}
