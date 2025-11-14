using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Domain.DTOs.Account;
using ProjectManager.Domain.DTOs.Identity;
using ProjectManager.Domain.Entities;
using ProjectManager_API.Common;
using ProjectManager.Application.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManager_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            UserManager<User> userManager,
            ITokenService tokenService,
            SignInManager<User> signInManager,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _logger = logger;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<NewUserDto>>> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            if (!ModelState.IsValid)
                throw new ValidationException("Invalid input data");

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed — user not found: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login failed — email not confirmed: {Email}", loginDto.Email);
                throw new UnauthorizedException("Please confirm your email before logging in.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed — wrong password for email: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            var tokens = await _tokenService.CreateToken(user);

            var newUser = new NewUserDto
            {
                Id = Guid.Parse(user.Id),
                UserName = user.UserName,
                Email = user.Email,
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            };

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);

            return Ok(ApiResponseFactory.Success(newUser, "Login successful"));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<NewUserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

            if (!ModelState.IsValid)
                throw new ValidationException("Invalid registration data");

            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                _logger.LogWarning("Registration failed — email already exists: {Email}", registerDto.Email);
                throw new ValidationException("User with this email already exists");
            }

            var appUser = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createdUser.Succeeded)
            {
                _logger.LogError("User creation failed: {Errors}",
                    string.Join("; ", createdUser.Errors.Select(e => e.Description)));

                throw new ValidationException(string.Join("; ", createdUser.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to assign role to user {Email}. Rolling back...", registerDto.Email);
                await _userManager.DeleteAsync(appUser);
                throw new Exception("Failed to assign role to the user");
            }

            var tokens = await _tokenService.CreateToken(appUser);

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
            var confirmationLink = $"https://localhost:7037/api/email/confirm?userId={appUser.Id}&token={encodedToken}";

            await _emailService.SendEmailConfirmationAsync(appUser.Email, confirmationLink);

            _logger.LogInformation("Email confirmation sent to {Email}", registerDto.Email);   

            _logger.LogInformation("User {Email} registered successfully", registerDto.Email);

            var newUser = new NewUserDto
            {
                Id = Guid.Parse(appUser.Id),
                UserName = appUser.UserName,
                Email = appUser.Email,
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            };

            return Ok(ApiResponseFactory.Created(newUser, "User registered successfully"));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<NewUserDto>>> Refresh([FromBody] RefreshRequestDto dto)
        {
            _logger.LogInformation("Token refresh attempt");

            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

            if (user == null)
            {
                _logger.LogWarning("Refresh failed — user not found by refresh token");
                throw new UnauthorizedException("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh failed — refresh token expired for user {UserId}", user.Id);
                throw new UnauthorizedException("Invalid refresh token");
            }

            var tokens = await _tokenService.CreateToken(user);

            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiryTime = tokens.RefreshTokenExpiryTime;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Refresh token updated for user {UserId}", user.Id);

            return Ok(ApiResponseFactory.Success(tokens, "Token refreshed successfully"));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] RefreshRequestDto dto)
        {
            _logger.LogInformation("Logout attempt");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Logout failed — refresh token not found");
                throw new ValidationException("Invalid refresh token");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to logout user {UserId}", user.Id);
                throw new Exception("Failed to logout user");
            }

            _logger.LogInformation("User {UserId} logged out successfully", user.Id);

            return Ok(ApiResponseFactory.NoContent());
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            _logger.LogInformation("Account deletion attempt for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Account deletion failed — user not found: {UserId}", userId);
                throw new NotFoundException("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to delete user {UserId}: {Errors}",
                    userId,
                    string.Join("; ", result.Errors.Select(e => e.Description)));

                throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("User {UserId} deleted account successfully", userId);

            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
