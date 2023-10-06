using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Shop.Configurations;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Cms;

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
            int maxConnectionAttempts = 3; // Максимальное количество попыток соединения
            int currentConnectionAttempt = 0;

            while (!_smtpClient.IsConnected)
            {
                try
                {
                    currentConnectionAttempt++;
                    await _smtpClient.ConnectAsync(_smtpConfig.SmtpServer, _smtpConfig.SmtpPort, SecureSocketOptions.None);
                    _logger.LogInformation("Соединение установлено.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Не удалось установить соединение. Попытка {currentConnectionAttempt}/{maxConnectionAttempts}");
                    currentConnectionAttempt++;
                    if (currentConnectionAttempt < maxConnectionAttempts)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(30));
                        continue;
                    }
                    _logger.LogError(ex, $"Не удалось установить соединение. Попытки исчерпаны");
                    throw;
                }
            }

            if (_smtpClient.IsConnected) //проверяем установлено ли соединение, перед аутентификацией
            {
                try
                {
                    if (!_smtpClient.IsAuthenticated)
                    {
                        await _smtpClient.AuthenticateAsync(_smtpConfig.Address, _smtpConfig.Password);
                        _logger.LogInformation("Аутентификация пройдена успешно.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Аутентификация не пройдена. Проверьте данные и повторите попытку");
                    throw;
                }
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

            //var attempt = 0;
            //while (attempt < _smtpConfig.MaxRetryAttempts)
            //{
            //    try
            //    {
            //        await _smtpClient.SendAsync(emailMessage);
            //        _logger.LogInformation("Письмо успешно отправлено: {Recipient}, {Subject}", recipient, subject);
            //        break;
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogWarning(ex, "Ошибка при отправке письма: {Recipient}, {Subject}", recipient, subject);
            //        attempt++;
            //        if (attempt < _smtpConfig.MaxRetryAttempts)
            //        {
            //            await Task.Delay(TimeSpan.FromSeconds(30)); // Задержка перед повторной попыткой
            //            _logger.LogInformation("Повторная попытка отправки письма: {Recipient}, {Subject}", recipient, subject);
            //            continue;
            //        }
            //        _logger.LogError(ex, "Произошла ошибка. Письмо не отправлено: {Recipient}, {Subject}", recipient, subject);

            //    }
            //}
        }

        public async ValueTask DisposeAsync()
        {
            await _smtpClient.DisconnectAsync(true);
            _smtpClient.Dispose();
        }
    }
}
