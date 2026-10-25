namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }


}
