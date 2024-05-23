using Azure;
using Azure.Communication.Email;
using Beacon.API.Settings;
using Microsoft.Extensions.Options;

namespace Beacon.API.Services;

public interface IEmailService
{
    Task<IEmailSendOperation?> SendAsync(string subject, string htmlBody, string toAddress);
}

public interface IEmailSendOperation
{
    string OperationId { get; }
    DateTime Timestamp { get; }
}

internal sealed class EmailService(IOptions<EmailSettings> settings) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task<IEmailSendOperation?> SendAsync(string subject, string htmlBody, string toAddress)
    {
        //var subject = "Welcome to Azure Communication Service Email APIs.";
        //var htmlContent = "<html><body><h1>Quick send email test</h1><br/><h4>This email message is sent from Azure Communication Service Email.</h4><p>This mail was sent using .NET SDK!!</p></body></html>";

        try
        {
            var emailClient = new EmailClient(_settings.ConnectionString);

            var sendEmailTask = emailClient.SendAsync(
                WaitUntil.Started,
                _settings.MailFrom,
                toAddress,
                subject,
                $"<html><body>{htmlBody}</body></html>");

            return new BeaconEmailSendOperation(await sendEmailTask);
        }
        catch (RequestFailedException ex)
        {
            // OperationID is contained in the exception message and can be used for troubleshooting purposes
            Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
            return null;
        }
    }
}

internal class BeaconEmailSendOperation(EmailSendOperation emailSendOperation) : IEmailSendOperation
{
    private readonly EmailSendOperation _emailSendOperation = emailSendOperation;

    public string OperationId => _emailSendOperation.Id;
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
