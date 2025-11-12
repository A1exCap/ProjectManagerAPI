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
using ProjectManager_API.Exceptions;
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
                throw new UnauthorizedException("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid email or password");

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
                throw new ValidationException("User with this email already exists");

            var appUser = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createdUser.Succeeded)
                throw new ValidationException(string.Join("; ", createdUser.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser);
                throw new Exception("Failed to assign role to the user");
            }

            var tokens = await _tokenService.CreateToken(appUser);

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
            var confirmationLink = $"https://localhost:7037/api/email/confirm?userId={appUser.Id}&token={encodedToken}";
            await _emailService.SendEmailConfirmationAsync(appUser.Email, confirmationLink);

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
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid refresh token");

            var tokens = await _tokenService.CreateToken(user);

            return Ok(ApiResponseFactory.Success(tokens, "Token refreshed successfully"));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] RefreshRequestDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
            if (user == null)
                throw new ValidationException("Invalid refresh token");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Failed to logout user");

            return Ok(ApiResponseFactory.Success<object>(null, "Logged out successfully"));
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return Ok(ApiResponseFactory.Success<object>(null, "Account deleted successfully"));
        }
    }
}
