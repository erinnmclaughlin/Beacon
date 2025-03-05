namespace Beacon.Common;

public static class DateExtensions
{
    public static DateOnly ToDateOnly(this DateTimeOffset date) => date.DateTime.ToDateOnly();
    public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);
    
    public static DateTime ToDateTime(this DateOnly date) => new(date, TimeOnly.MinValue, DateTimeKind.Utc);
    public static DateTimeOffset ToDateTimeOffset(this DateOnly date) => new(date, TimeOnly.MinValue, TimeSpan.Zero);
}