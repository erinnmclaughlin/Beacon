namespace Beacon.Common.Services;

public interface ILabContext
{
    CurrentUser CurrentUser { get; }
    CurrentLab CurrentLab { get; }
}
