namespace Beacon.App.Services;

public interface IEmailSendOperation
{
    string OperationId { get; }
    DateTimeOffset Timestamp { get; }
}
