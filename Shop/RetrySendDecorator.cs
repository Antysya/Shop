using Shop.Configurations;

namespace Shop
{
    public class RetrySendDecorator : ISmtpEmailSender
    {
        private readonly ISmtpEmailSender _inner;
        private readonly ILogger<RetrySendDecorator> _logger;

        public RetrySendDecorator(ISmtpEmailSender inner, ILogger<RetrySendDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task SendEmailAsync(string recipient, string subject, string message)
        {
            int maxRetryAttempts = 3;
            int currentSendingAttempt = 0;

            while (currentSendingAttempt < maxRetryAttempts)
            {
                try
                {
                    _logger.LogInformation("Попытка отправить письмо: {Recipient}, {Subject}", recipient, subject);
                    await _inner.SendEmailAsync(recipient, subject, message);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ошибка при отправки письма: {Recipient}, {Subject}", recipient, subject);
                }

                currentSendingAttempt++;
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
            throw new Exception("Произошла ошибка. Письмо не отправлено!");
        }
    }
}