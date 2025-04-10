using IS_Kactus_Expenses.Service.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace IS_Kactus_Expenses.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]!),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]!)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:From"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
