namespace Beacon.API.App.Services.Email;

public interface IEmailSendOperation
{
    string OperationId { get; }
    DateTimeOffset Timestamp { get; }
}
