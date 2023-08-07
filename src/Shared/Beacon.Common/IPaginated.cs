namespace Beacon.Common;

public interface IPaginated
{
    public int PageNumber { get; }
    public int PageSize { get; }
}