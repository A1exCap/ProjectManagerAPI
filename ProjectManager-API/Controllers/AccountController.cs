using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Domain.DTOs.Account;
using ProjectManager.Domain.DTOs.Identity;
using ProjectManager.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManager_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, IEmailService emailService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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
                    Id = Guid.Parse(user.Id),
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

                        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
                        var confirmationLink = $"https://localhost:7037/api/email/confirm?userId={appUser.Id}&token={encodedToken}";

                        await _emailService.SendEmailConfirmationAsync(appUser.Email, confirmationLink);

                        return Ok(
                            new NewUserDto
                            {                    
                                Id = Guid.Parse(appUser.Id),
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
                    await _userManager.DeleteAsync(appUser);
                    return BadRequest(createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Account deleted successfully.");
        }
    }
}
