using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Shop.Configurations;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Shop
{
    #region
    /// <summary>
    /// ДЗ_5
    /// </summary>
    //public class EmailService : IEmailService, IDisposable
    //{
    //    private readonly SmtpClient _smtpClient;

    //    public EmailService()
    //    {
    //        _smtpClient = new SmtpClient();
    //        _smtpClient.Connect("smtp.beget.com", 25, SecureSocketOptions.None);
    //        _smtpClient.Authenticate("asp2022pd011@rodion-m.ru", "6WU4x2be");
    //    }

    //    public Task SendEmailAsync(string recipient, string subject, string message)
    //    {
    //        var emailMessage = new MimeMessage();
    //        emailMessage.From.Add(new MailboxAddress("Anna E.", "asp2022pd011@rodion-m.ru"));
    //        emailMessage.To.Add(new MailboxAddress("", recipient));
    //        emailMessage.Subject = subject;
    //        emailMessage.Body = new TextPart("plain")
    //        {
    //            Text = message
    //        };

    //        _smtpClient.Send(emailMessage);

    //        return Task.CompletedTask;
    //    }

    //    public void Dispose()
    //    {
    //        _smtpClient.Disconnect(true);
    //        _smtpClient.Dispose();
    //    }
    //}
    #endregion

    //ДЗ_6
    public class SmtpEmailSender : ISmtpEmailSender, IAsyncDisposable
    {
        //private readonly string smtpServer = "smtp.beget.com";
        //private readonly int smtpPort = 25;
        //private readonly string address = "asp2022pd011@rodion-m.ru";
        //private readonly string userName = "Anna E.";
        //private readonly string password = "6WU4x2be"; 
        private readonly SmtpClient _smtpClient = new();
        private readonly SmtpConfig _smtpConfig;

        //public SmtpEmailSender(IOptions<SmtpConfig> options)
        //{
        //    ArgumentNullException.ThrowIfNull(options);
        //    _smtpConfig = options.Value;
        //}

        public SmtpEmailSender(IOptionsSnapshot<SmtpConfig> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            _smtpConfig = options.Value;
        }

        private async Task EnsureConnectedAndAuthenticated()
        {
            if (!_smtpClient.IsConnected)
            {
                await _smtpClient.ConnectAsync(_smtpConfig.SmtpServer, _smtpConfig.SmtpPort, SecureSocketOptions.None);
            }
            if (!_smtpClient.IsAuthenticated)
            {
                await _smtpClient.AuthenticateAsync(_smtpConfig.Address, _smtpConfig.Password);
            }
        }

        public async Task SendEmailAsync(string recipient, string subject, string message)
        {
            await EnsureConnectedAndAuthenticated();
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_smtpConfig.UserName, _smtpConfig.Address));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            await _smtpClient.SendAsync(emailMessage);
        }

        public async ValueTask DisposeAsync()
        {
            await _smtpClient.DisconnectAsync(true);
            _smtpClient.Dispose();
        }
    }
}
