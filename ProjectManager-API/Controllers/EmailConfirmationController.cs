using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.WebEncoders;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Domain.DTOs.Identity;
using ProjectManager.Domain.Entities;
using System.Text;

namespace ProjectManager_API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailConfirmationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public EmailConfirmationController(UserManager<User> userManager, IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest("Invalid user ID");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return BadRequest("Invalid or expired confirmation token.");       
            
            return Ok("Email confirmed successfully.");
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendEmailDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("User with this email does not exist.");

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email is already confirmed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = $"https://localhost:7037/api/email/confirm?userId={user.Id}&token={encodedToken}";

            await _emailService.SendEmailConfirmationAsync(dto.Email, confirmationLink);

            return Ok("Confirmation email has been resent.");
        }
    }
}
