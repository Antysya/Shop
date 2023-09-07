using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace Shop
{
    public class EmailService : IEmailService, IDisposable
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Connect("smtp.beget.com", 25, SecureSocketOptions.None);
            _smtpClient.Authenticate("asp2022pd011@rodion-m.ru", "6WU4x2be");
        }

        public Task SendEmailAsync(string recipient, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Anna E.", "asp2022pd011@rodion-m.ru"));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            _smtpClient.Send(emailMessage);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }
    }
}
