namespace Beacon.App.Services;

public interface IEmailService
{
    Task<IEmailSendOperation?> SendAsync(string subject, string htmlBody, string toAddress);
}
