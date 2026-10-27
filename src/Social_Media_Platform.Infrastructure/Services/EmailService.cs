using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Infrastructure.Options;
using System.Net;
using System.Net.Mail;

namespace Social_Media_Platform.Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
        {
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public async Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            var host = _emailOptions.Smtp.Host ?? "smtp.gmail.com";
            var port = _emailOptions.Smtp.Port > 0 ? _emailOptions.Smtp.Port : 587;
            var username = _emailOptions.Smtp.Username;
            var password = _emailOptions.Smtp.Password;
            var from = _emailOptions.Smtp.From ?? username;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("SMTP username and password must be configured.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(from!),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(recipient));

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await smtp.SendMailAsync(message, cancellationToken);

                _logger.LogInformation("Email sent successfully to {Recipient}", recipient);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Email sending was canceled for {Recipient}", recipient);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", recipient);

                throw new InvalidOperationException("Failed to send email. Check SMTP settings.", ex);
            }
        }
    }

}
