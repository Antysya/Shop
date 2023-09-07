namespace Shop
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipient, string subject, string message);
    }
}
