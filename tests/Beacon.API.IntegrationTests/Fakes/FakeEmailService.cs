using Beacon.App.Services;

namespace Beacon.API.IntegrationTests.Fakes;

public class FakeEmailService : IEmailService
{
    public async Task<IEmailSendOperation?> SendAsync(string subject, string htmlBody, string toAddress)
    {
        return await Task.FromResult(new FakeEmailSendOperation());
    }
}

public class FakeEmailSendOperation : IEmailSendOperation
{
    public const string OperationId = "12345";

    string IEmailSendOperation.OperationId => "12345";
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}
