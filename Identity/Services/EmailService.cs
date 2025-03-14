using Domain.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace Identity.Services
{
    public class EmailService : IEmailService
    {
      
            private readonly EmailSettings _emailSettings;

            public EmailService(IOptions<EmailSettings> emailSettings)
            {
                _emailSettings = emailSettings.Value;
            }

            public async Task SendEmailAsync(string email, string subject, string message)
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }

    public class EmailSettings
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string SmtpServer { get; set; }
        public required int Port { get; set; }
    }

}
