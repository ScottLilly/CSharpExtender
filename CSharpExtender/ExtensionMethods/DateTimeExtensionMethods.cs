using System;
using System.Globalization;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Scott's extension methods for DateTime
    /// </summary>
    public static class DateTimeExtensionMethods
    {
        private static readonly TimeSpan s_endOfDay = new TimeSpan(23, 59, 59);

        /// <summary>
        /// Returns the start of the day 00:00:00 for the provided date.
        /// </summary>
        /// <param name="date">The date to get the start of the day from.</param>
        /// <returns>The start of the day for the provided date.</returns>
        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// Returns the end of the day 23:59:59 for the provided date.
        /// </summary>
        /// <param name="date">The date to get the end of the day from.</param>
        /// <returns>The end of the day for the provided date.</returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.Add(s_endOfDay);
        }

        /// <summary>
        /// Returns the date for the start of the month, at 00:00:00, for the provided date.
        /// </summary>
        /// <param name="date">The date to get the start of the month from.</param>
        /// <returns>The start of the month for the provided date.</returns>
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Returns the date for the end of the month, at 23:59:59, for the provided date.
        /// </summary>
        /// <param name="date">The date to get the end of the month from.</param>
        /// <returns>The end of the month for the provided date.</returns>
        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month,
                DateTime.DaysInMonth(date.Year, date.Month)).Add(s_endOfDay);
        }

        /// <summary>
        /// Converts the provided date to an ISO 8601 string.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>The ISO 8601 string representation of the provided date.</returns>
        public static string ToIso8601String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ",
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks if the given date is a weekend.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the date is a weekend, false otherwise.</returns>
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Checks if the given date is a weekday.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the date is a weekday, false otherwise.</returns>
        public static bool IsWeekday(this DateTime date)
        {
            return !date.IsWeekend();
        }

        /// <summary>
        /// Gets the next day from the given date.
        /// </summary>
        /// <param name="date">The date to get the next day from.</param>
        /// <returns>The next day from the given date.</returns>
        public static DateTime NextDay(this DateTime date)
        {
            return date.AddDays(1);
        }

        /// <summary>
        /// Gets the previous day from the given date.
        /// </summary>
        /// <param name="date">The date to get the previous day from.</param>
        /// <returns>The previous day from the given date.</returns>
        public static DateTime PreviousDay(this DateTime date)
        {
            return date.AddDays(-1);
        }
    }
}