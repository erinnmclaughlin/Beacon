namespace Beacon.Common;

public static class DateExtensions
{
    public static DateOnly ToDateOnly(this DateTimeOffset date) => date.DateTime.ToDateOnly();
    public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);
    
    public static TimeOnly ToTimeOnly(this DateTimeOffset date) => date.DateTime.ToTimeOnly();
    public static TimeOnly ToTimeOnly(this DateTime date) => TimeOnly.FromDateTime(date);
    
    public static DateTime ToDateTime(this DateOnly date) => new(date, TimeOnly.MinValue, DateTimeKind.Utc);
    public static DateTimeOffset ToDateTimeOffset(this DateOnly date) => date.ToDateTimeOffset(TimeOnly.MinValue);
    public static DateTimeOffset ToDateTimeOffset(this DateOnly date, TimeOnly timeOnly) => new(date, timeOnly, TimeSpan.Zero);
    
    public static DateTimeOffset WithNewTimePart(this DateTimeOffset date, TimeOnly timePart) => new(date.ToDateOnly(), timePart, date.Offset);
}