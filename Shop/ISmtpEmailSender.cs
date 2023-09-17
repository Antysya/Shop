namespace Shop
{
    public interface ISmtpEmailSender
    {
        Task SendEmailAsync(string recipient, string subject, string message);
    }
}
