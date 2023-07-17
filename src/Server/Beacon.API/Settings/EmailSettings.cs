namespace Beacon.API.Settings;

public sealed class EmailSettings
{
    public required string ConnectionString { get; init; }
    public required string MailFrom { get; init; }
}
