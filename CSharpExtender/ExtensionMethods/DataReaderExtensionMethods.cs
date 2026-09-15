using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for IDataReader, reading a column by name instead of by ordinal.
/// </summary>
/// <remarks>
/// The typed methods here share their names with the ordinal-based ones IDataReader
/// declares itself, such as <see cref="IDataRecord.GetString(int)"/>. Overload resolution
/// separates them by argument type, so a string argument reaches these and an int argument
/// reaches the reader's own.
/// </remarks>
public static class DataReaderExtensionMethods
{
    /// <summary>
    /// Get a column value from an IDataReader, by column name.
    /// </summary>
    /// <typeparam name="T">Type of the returned value.</typeparam>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>
    /// The value of the column, or default when the column holds DBNull. A nullable
    /// <typeparamref name="T"/> is converted to its underlying type, so a null column read
    /// as int? returns null rather than throwing.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="reader"/> or <paramref name="columnName"/> is null.
    /// </exception>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="columnName"/> is not a column in the result set. The
    /// message names the column and lists the ones that are there.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the column's value cannot be converted to <typeparamref name="T"/>. The
    /// message names the column, which is the whole point of reading by name.
    /// </exception>
    public static T GetValue<T>(this IDataReader reader, string columnName)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(columnName);

        int ordinal = OrdinalOf(reader, columnName);

        if (reader.IsDBNull(ordinal))
        {
            return default;
        }

        object value = reader.GetValue(ordinal);

        if (value is T typedValue)
        {
            return typedValue;
        }

        // ChangeType cannot target Nullable<T>, and the DBNull case above already
        // answered the only question the nullability was asking
        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        try
        {
            // Guid has no IConvertible path, so a column storing one as text would
            // fail ChangeType even though the conversion is obvious
            if (targetType == typeof(Guid))
            {
                return (T)(object)Guid.Parse(value.ToString());
            }

            // ChangeType covers the numeric, string, bool and DateTime conversions.
            // Invariant culture because a database is a culture-neutral store: a
            // decimal read as text should not depend on the machine's separator.
            return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (Exception exception) when (
            exception is InvalidCastException ||
            exception is FormatException ||
            exception is OverflowException ||
            exception is ArgumentException)
        {
            throw new InvalidCastException(
                $"Column '{columnName}' holds a {value.GetType().Name} of '{value}', " +
                $"which cannot be converted to {typeof(T).Name}.",
                exception);
        }
    }

    /// <summary>
    /// Get a column value as a string, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or null when it holds DBNull.</returns>
    public static string GetString(this IDataReader reader, string columnName) =>
        reader.GetValue<string>(columnName);

    /// <summary>
    /// Get a column value as an int, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or zero when it holds DBNull.</returns>
    public static int GetInt32(this IDataReader reader, string columnName) =>
        reader.GetValue<int>(columnName);

    /// <summary>
    /// Get a column value as a bool, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or false when it holds DBNull.</returns>
    public static bool GetBoolean(this IDataReader reader, string columnName) =>
        reader.GetValue<bool>(columnName);

    /// <summary>
    /// Get a column value as a decimal, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or zero when it holds DBNull.</returns>
    public static decimal GetDecimal(this IDataReader reader, string columnName) =>
        reader.GetValue<decimal>(columnName);

    /// <summary>
    /// Get a column value as a double, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or zero when it holds DBNull.</returns>
    public static double GetDouble(this IDataReader reader, string columnName) =>
        reader.GetValue<double>(columnName);

    /// <summary>
    /// Get a column value as a float, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or zero when it holds DBNull.</returns>
    public static float GetFloat(this IDataReader reader, string columnName) =>
        reader.GetValue<float>(columnName);

    /// <summary>
    /// Get a column value as a DateTime, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or DateTime.MinValue when it holds DBNull.</returns>
    public static DateTime GetDateTime(this IDataReader reader, string columnName) =>
        reader.GetValue<DateTime>(columnName);

    /// <summary>
    /// Get a column value as a Guid, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>
    /// The value of the column, or Guid.Empty when it holds DBNull. A column storing the
    /// value as text is parsed.
    /// </returns>
    public static Guid GetGuid(this IDataReader reader, string columnName) =>
        reader.GetValue<Guid>(columnName);

    /// <summary>
    /// Get a column value as a byte array, by column name.
    /// </summary>
    /// <param name="reader">IDataReader being read from.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The value of the column, or null when it holds DBNull.</returns>
    /// <remarks>
    /// Named for the type rather than GetBytes, because IDataReader already declares a
    /// GetBytes that copies into a caller's buffer and returns how many bytes it read.
    /// </remarks>
    public static byte[] GetByteArray(this IDataReader reader, string columnName) =>
        reader.GetValue<byte[]>(columnName);

    #region Private Methods

    // GetOrdinal is documented to throw IndexOutOfRangeException for a name that is not
    // in the result set, but implementations disagree: DataTableReader throws
    // ArgumentException instead. Normalizing it keeps the contract the same whichever
    // provider is behind the interface, and the message says which column was asked for
    // rather than leaving the caller to guess.
    private static int OrdinalOf(IDataReader reader, string columnName)
    {
        try
        {
            return reader.GetOrdinal(columnName);
        }
        catch (Exception exception) when (
            exception is IndexOutOfRangeException || exception is ArgumentException)
        {
            throw new IndexOutOfRangeException(
                $"'{columnName}' is not a column in this result set. " +
                $"The columns are: {ColumnNames(reader)}.",
                exception);
        }
    }

    private static string ColumnNames(IDataReader reader) =>
        string.Join(", ", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName));

    #endregion
}
