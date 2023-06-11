using Beacon.App.Services;

namespace Beacon.IntegrationTests;

public class FakeEmailService : IEmailService
{
    public async Task<IEmailSendOperation?> SendAsync(string subject, string htmlBody, string toAddress)
    {
        return await Task.FromResult(new FakeEmailSendOperation());
    }
}

public class FakeEmailSendOperation : IEmailSendOperation
{
    public string OperationId => "12345";
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}
