using MailKit.Security;
using MechanicShop.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MechanicShop.Infrastructure.Services
{
    internal class NotificationService(ILogger<NotificationService> logger,IConfiguration configuration) : INotificationService
    {
        private readonly ILogger<NotificationService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            // Read email settings from configuration
            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpHost = emailSettings["SmtpHost"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]!);
            var smtpUser = emailSettings["SmtpUser"];
            var smtpPass = emailSettings["SmtpPass"];
            var fromName = emailSettings["FromName"];
            var fromEmail = emailSettings["FromEmail"];


            // create the email message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(fromName, fromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body }; // HTML email

            // send the email by MailKit
            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(smtpUser, smtpPass, cancellationToken);
            await client.SendAsync(email, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        public async Task SendSmsAsync(string phoneNumber, string message,CancellationToken cancellationToken = default)
        {
            var masked = phoneNumber.Length >= 4
          ? new string('*', phoneNumber.Length - 4) + phoneNumber[^4..]
          : "****";

            _logger.LogInformation("[SMS] To: {Phone} | Message: {Message}", masked, message);

            // Simulated SMS send
            await Task.CompletedTask;
        }
    }
}
