namespace RealtyHub.Core.Extensions;

public static class DateTimeExtension
{
    public static DateTime GetFirstDay(this DateTime date, int? year = null, int? month = null)
        => new(year ?? date.Year, month ?? date.Month, 1);

    public static DateTime GetLastDay(
        this DateTime date, int? year = null, int? month = null)
        => new DateTime(
                year ?? date.Year,
                month ?? date.Month,
                1)
            .AddMonths(1)
            .AddDays(-1);

    public static string ToUtcString(this DateTime? date) =>
        date is null 
            ? string.Empty 
            : date.Value.ToUniversalTime().ToString("o");

    public static DateTime ToEndOfDay(this DateTime date) =>
        date.Date
            .AddHours(23)
            .AddMinutes(59)
            .AddSeconds(59);
}