using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Shop.Configurations;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Shop
{
    public class SmtpEmailSender : ISmtpEmailSender, IAsyncDisposable
    {
        private readonly SmtpClient _smtpClient = new();
        private readonly SmtpConfig _smtpConfig;
        private readonly ILogger<SmtpEmailSender> _logger; // добавляем логгер

        public SmtpEmailSender(IOptionsSnapshot<SmtpConfig> options, ILogger<SmtpEmailSender> logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            _smtpConfig = options.Value;
            _logger = logger;
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
            
            _logger.LogInformation("Попытка отправить письмо: {Recipient}, {Subject}", recipient, subject);
            var attempt = 0;
            while (attempt < _smtpConfig.MaxRetryAttempts)
            {
                try
                {
                    await _smtpClient.SendAsync(emailMessage);
                    _logger.LogInformation("Письмо успешно отправлено: {Recipient}, {Subject}", recipient, subject);
                    break; 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при отправке письма: {Recipient}, {Subject}", recipient, subject);
                    attempt++;
                    if (attempt < _smtpConfig.MaxRetryAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30)); // Задержка перед повторной попыткой
                        _logger.LogInformation("Повторная попытка отправки письма: {Recipient}, {Subject}", recipient, subject);
                        continue;
                    }
                    _logger.LogError(ex, "Произошла ошибка. Письмо не отправлено: {Recipient}, {Subject}", recipient, subject);

                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _smtpClient.DisconnectAsync(true);
            _smtpClient.Dispose();
        }
    }
}
