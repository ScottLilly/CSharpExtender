namespace CSharpExtender.ExtensionMethods;

public static class DateTimeExtensionMethods
{
    public static DateTime StartOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }

    public static DateTime EndOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
    }

    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
    }

    public static DateTime EndOfMonth(this DateTime date)
    {
        return date.StartOfMonth().AddMonths(1).AddSeconds(-1);
    }

    public static string ToIso8601String(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}