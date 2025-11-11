using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ProjectManager.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ProjectManager.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("My API", _config["Email:From"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Confirm your email";

            message.Body = new TextPart("html")
            {
                Text = $"<h2>Confirm Your Email</h2><p>Click the link below:</p><a href='{confirmationLink}'>Confirm Email</a>"
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_config["Email:Smtp"], 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["Email:From"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
