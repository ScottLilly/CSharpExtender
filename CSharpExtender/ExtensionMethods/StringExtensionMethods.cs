using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for strings
/// </summary>
public static class StringExtensionMethods
{
    /// <summary>
    /// Check if strings are equal, using InvariantCultureIgnoreCase
    /// </summary>
    /// <param name="text"></param>
    /// <param name="matchingText"></param>
    /// <returns>True, if string match. False, if they don't.</returns>
    public static bool Matches(this string text, string matchingText)
    {
        return text.Equals(matchingText, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Check if strings are not equal, using InvariantCultureIgnoreCase
    /// </summary>
    /// <param name="text"></param>
    /// <param name="comparisonText"></param>
    /// <returns>True, if strings do not match. False, if they do.</returns>
    public static bool DoesNotMatch(this string text, string comparisonText)
    {
        return !text.Matches(comparisonText);
    }

    /// <summary>
    /// Returns 'true' if the string is not null, empty or only contains whitespace
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool HasText(this string value) =>
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Returns 'true' if the string is null, empty or only contains whitespace
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool DoesNotHaveText(this string value) =>
        string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Returns a null if the string is null, empty or only contains whitespace
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string NullIfEmpty(this string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>
    /// Returns a string with all non-digits removed
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToDigitsOnly(this string value) =>
        string.IsNullOrWhiteSpace(value)
        ? null
        : new string(value.Where(char.IsDigit).ToArray());

    /// <summary>
    /// Returns 'true' is the string only contains digits
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsDigitsOnly(this string s)
    {
        ReadOnlySpan<char> span = s.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsDigit(span[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Converts an IEnumerable of strings to a single string with line feeds between each string
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static string ToStringWithLineFeeds(this IEnumerable<string> lines)
    {
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Returns a string with the text repeated the specified number of times
    /// </summary>
    /// <param name="text"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public static string Repeat(this string text, int times)
    {
        if (times < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(times), "Must be zero or greater.");
        }

        return string.Concat(Enumerable.Repeat(text, times));
    }

    /// <summary>
    /// Convert a file path into an array of the individual directories.
    /// Handles both forward and double back slashes.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>
    /// An array of trimmed strings, split by the path separator characters.
    /// Does not include any empty entries.
    /// </returns>
    public static IEnumerable<string> SplitPath(this string path)
    {
        // Manually trim the split strings as StringSplitOptions.TrimEntries is not available
        return path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim());
    }

    /// <summary>
    /// Checks if a string contains all the words in the specified array.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="requiredWords"></param>
    /// <returns></returns>
    public static bool IncludesTheWords(this string text, params string[] requiredWords)
    {
        if (string.IsNullOrWhiteSpace(text) ||
            requiredWords.Length == 0 ||
            requiredWords.All(string.IsNullOrWhiteSpace))
        {
            return false;
        }

        // TODO: Verifiy this handles punctuation
        // TODO: Accept a StringComparison parameter
        return requiredWords
            .All(word => text.Contains(word, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Removes all instances of the specified text from the string.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="textToRemove"></param>
    /// <param name="stringComparisonMethod"></param>
    /// <returns></returns>
    public static string RemoveText(this string text, string textToRemove,
        StringComparison stringComparisonMethod = StringComparison.CurrentCultureIgnoreCase)
    {
        if (text.DoesNotHaveText() || textToRemove.DoesNotHaveText())
        {
            return text;
        }

        while (text.Contains(textToRemove, stringComparisonMethod))
        {
            text = text.Replace(textToRemove, "", stringComparisonMethod);
        }

        return text;
    }

    /// <summary>
    /// Converts a string to a specified generic type T.
    /// </summary>
    /// <typeparam name="T">The type to convert the string to.</typeparam>
    /// <param name="input">The string to convert.</param>
    /// <returns>The converted value of type T.</returns>
    /// <exception cref="NotSupportedException">Thrown if conversion is not supported for the type.</exception>
    /// <exception cref="FormatException">Thrown if the string is not in a format compliant with the type.</exception>
    public static T ConvertFromString<T>(this string input)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

        if (converter != null && converter.CanConvertFrom(typeof(string)))
        {
            return (T)converter.ConvertFromString(input);
        }
        else
        {
            throw new NotSupportedException($"Conversion from string to type {typeof(T).Name} is not supported.");
        }
    }

    /// <summary>
    /// Splits a PascalCase string into list of words, based on locaiton of upper-case letters
    /// </summary>
    /// <param name="input">String to split</param>
    /// <returns>List of strings, split on the uppercase letters</returns>
    public static List<string> SplitPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new List<string>() { input };
        }

        return Regex.Replace(input, @"(?<!^)(?<![\W_])(?=[A-Z])", " ").Split(' ').ToList();
    }

    /// <summary>
    /// Safely trim a string to a maximumlength
    /// </summary>
    /// <param name="text">String to trim</param>
    /// <param name="maxLength">Maximum length of string</param>
    /// <returns>String, trimmed (if necessary) to maximum length</returns>
    /// <exception cref="ArgumentOutOfRangeException">Exception, if a negative number is passed as maxLength</exception>
    public static string ToMaxLengthOf(this string text, int maxLength)
    {
        if (text == null)
        {
            return null;
        }

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException("maxLength must be non-negative", nameof(maxLength));
        }

        return text.Length <= maxLength ? text : text.Substring(0, maxLength);
    }
}