using BookBagaicha.IService;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookBagaicha.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                // Get email settings from configuration
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var host = smtpSettings["Host"];
                var port = int.Parse(smtpSettings["Port"]);
                var senderEmail = smtpSettings["SenderEmail"];
                var senderName = smtpSettings["SenderName"];
                var password = smtpSettings["Password"];
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

                // Create mail message
                var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                message.To.Add(to);

                // Create and configure SMTP client
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = enableSsl
                };

                // Send email
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", to, subject);
                throw;
            }
        }
    }
}