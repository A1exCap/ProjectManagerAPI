using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.WebEncoders;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Domain.DTOs.Identity;
using ProjectManager.Domain.Entities;
using ProjectManager_API.Common;
using ProjectManager.Application.Exceptions;
using System.Text;

namespace ProjectManager_API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailConfirmationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailConfirmationController> _logger;

        public EmailConfirmationController(UserManager<User> userManager, IEmailService emailService, ILogger<EmailConfirmationController> logger)
        {
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("confirm")]
        public async Task<ActionResult<ApiResponse<object>>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            _logger.LogInformation("Email confirmation attempt for userId: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed — user not found: {UserId}", userId);
                throw new NotFoundException("Invalid user ID");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Email confirmation failed — invalid/expired token for userId: {UserId}", userId);
                throw new ValidationException("Invalid or expired confirmation token.");
            }

            _logger.LogInformation("Email confirmed successfully for userId: {UserId}", userId);
            return Ok(ApiResponseFactory.NoContent());
        }

        [HttpPost("resend")]
        public async Task<ActionResult<ApiResponse<object>>> ResendConfirmation([FromBody] ResendEmailDto dto)
        {
            _logger.LogInformation("Resend confirmation attempt for email: {Email}", dto.Email);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Resend confirmation failed — user not found: {Email}", dto.Email);
                throw new NotFoundException("User with this email does not exist.");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogInformation("Resend confirmation skipped — email already confirmed: {Email}", dto.Email);
                throw new ValidationException("Email is already confirmed.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = $"https://localhost:7037/api/email/confirm?userId={user.Id}&token={encodedToken}";
            await _emailService.SendEmailConfirmationAsync(dto.Email, confirmationLink);

            _logger.LogInformation("Confirmation email resent to: {Email}", dto.Email);
            return Ok(ApiResponseFactory.NoContent());
        }
    }
}
