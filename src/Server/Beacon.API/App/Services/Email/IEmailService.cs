namespace Beacon.API.App.Services.Email;

public interface IEmailService
{
    Task<IEmailSendOperation?> SendAsync(string subject, string htmlBody, string toAddress);
}
