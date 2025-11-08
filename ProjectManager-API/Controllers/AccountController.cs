using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Domain.DTOs.Account;
using ProjectManager.Domain.DTOs.Identity;
using ProjectManager.Domain.Entities;

namespace ProjectManager_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded || user == null)
                    return Unauthorized("Invalid email or password");

                var tokens = await _tokenService.CreateToken(user);

                return Ok(new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                    return BadRequest("User with this email already exists");

                var appUser = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        var tokens = await _tokenService.CreateToken(appUser);

                        return Ok(
                            new NewUserDto
                            {                     
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = tokens.AccessToken,
                                RefreshToken = tokens.RefreshToken
                            }
                        );
                    }
                    else
                    {
                        await _userManager.DeleteAsync(appUser);
                        return StatusCode(500, "An error occurred while assigning role to the user.");
                    }
                }

                else
                {
                    return BadRequest(createdUser.Errors);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

            if (user == null ||
                user.RefreshTokenExpiryTime == null ||
                user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            var tokens = await _tokenService.CreateToken(user);

            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

            if (user == null)
                return BadRequest("Invalid refresh token.");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return StatusCode(500, "Failed to logout user.");

            return Ok(new { message = "Logged out successfully." });
        }

    }
}
