using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for strings
/// </summary>
public static partial class StringExtensionMethods
{
    // Inputs at or below this length get a stack buffer instead of a heap array
    private const int _stackAllocLimit = 256;

    private static readonly char[] _pathSeparators = new char[] { '/', '\\' };

    // Source-generated, so the matcher is built at compile time
    [GeneratedRegex(@"(?<!^)(?<![\W_])(?=[A-Z])")]
    private static partial Regex PascalCaseBoundary();

    private static class ConverterFor<T>
    {
        internal static readonly TypeConverter Value = TypeDescriptor.GetConverter(typeof(T));
    }

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
    public static string ToDigitsOnly(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var source = value.AsSpan();

        // Sized to the input rather than to the limit, so only what is needed
        // gets zero-initialized
        Span<char> digits = source.Length <= _stackAllocLimit
            ? stackalloc char[source.Length]
            : new char[source.Length];

        int count = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (char.IsDigit(source[i]))
            {
                digits[count++] = source[i];
            }
        }

        return new string(digits.Slice(0, count));
    }

    /// <summary>
    /// Returns 'true' is the string only contains digits
    /// </summary>
    /// <param name="s">String to check. A null returns false.</param>
    /// <returns>True, if the string is not null and contains no non-digit characters.</returns>
    /// <remarks>
    /// An empty string returns true, because it contains no character that is not a
    /// digit. That reads as surprising and has been raised as such, but it is
    /// deliberate and is not going to change: the null case was the real defect and
    /// was fixed for 3.0.0. Test for emptiness separately if it matters to you.
    /// </remarks>
    public static bool IsDigitsOnly(this string s)
    {
        if (s == null)
        {
            return false;
        }

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

        if (times == 0 || string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // Repeating one character is common enough to be worth its own path
        if (text.Length == 1)
        {
            return new string(text[0], times);
        }

        return string.Create(text.Length * times, (text, times), (destination, state) =>
        {
            var source = state.text.AsSpan();

            for (int i = 0; i < state.times; i++)
            {
                source.CopyTo(destination.Slice(i * source.Length));
            }
        });
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
        // TrimEntries runs before RemoveEmptyEntries, so a segment of nothing but
        // whitespace is trimmed to empty and then dropped. Trimming afterwards
        // instead left it in the result as an empty string.
        return path.Split(_pathSeparators,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Checks if a string contains all the words in the specified array, comparing
    /// with CurrentCultureIgnoreCase.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="requiredWords">
    /// The words to look for. Each is matched as a substring rather than as a whole
    /// word, so punctuation around a word in the text does not stop it matching, and
    /// a word contained in a longer one counts as present.
    /// </param>
    /// <returns></returns>
    public static bool IncludesTheWords(this string text, params string[] requiredWords) =>
        text.IncludesTheWords(StringComparison.CurrentCultureIgnoreCase, requiredWords);

    /// <summary>
    /// Checks if a string contains all the words in the specified array.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="stringComparisonMethod">
    /// How to compare. Ordinal and OrdinalIgnoreCase are an order of magnitude
    /// faster than the culture-aware options, and are the right choice whenever the
    /// text is not natural language a person will read.
    /// </param>
    /// <param name="requiredWords">
    /// The words to look for. Each is matched as a substring rather than as a whole
    /// word, so punctuation around a word in the text does not stop it matching, and
    /// a word contained in a longer one counts as present.
    /// </param>
    /// <returns></returns>
    public static bool IncludesTheWords(this string text,
        StringComparison stringComparisonMethod, params string[] requiredWords)
    {
        if (string.IsNullOrWhiteSpace(text) || requiredWords.Length == 0)
        {
            return false;
        }

        bool anyWordHasText = false;

        for (int i = 0; i < requiredWords.Length; i++)
        {
            if (requiredWords[i].HasText())
            {
                anyWordHasText = true;
                break;
            }
        }

        if (!anyWordHasText)
        {
            return false;
        }

        var source = text.AsSpan();

        for (int i = 0; i < requiredWords.Length; i++)
        {
            if (source.IndexOf(requiredWords[i].AsSpan(), stringComparisonMethod) < 0)
            {
                return false;
            }
        }

        return true;
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

        // Removing one instance can create another ("aabb" minus "ab" leaves "ab"),
        // so this repeats until a pass changes nothing. Replacing with an empty
        // string can only shorten, and textToRemove is known non-empty by here, so
        // an unchanged length means the pass found nothing. Testing that rather
        // than calling Contains first saves a scan of the string per pass.
        while (true)
        {
            string shortened = text.Replace(textToRemove, "", stringComparisonMethod);

            if (shortened.Length == text.Length)
            {
                return text;
            }

            text = shortened;
        }
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
        // Held per closed T, because the converter for a type never changes and
        // asking TypeDescriptor for it again is most of what this method costs
        TypeConverter converter = ConverterFor<T>.Value;

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
    /// <param name="input">String to split. A null or empty string returns an empty list.</param>
    /// <returns>List of strings, split on the uppercase letters</returns>
    public static List<string> SplitPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new List<string>();
        }

        return PascalCaseBoundary().Replace(input, " ").Split(' ').ToList();
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
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Must be zero or greater.");
        }

        return text.Length <= maxLength ? text : text.Substring(0, maxLength);
    }

    /// <summary>
    /// Replaces the middle of a string with a mask character, leaving a number of
    /// characters visible at each end.
    /// </summary>
    /// <param name="text">String to mask. A null or empty string is returned unchanged.</param>
    /// <param name="maskChar">Character to replace each masked character with.</param>
    /// <param name="visiblePrefixLength">How many characters to leave visible at the start.</param>
    /// <param name="visibleSuffixLength">How many characters to leave visible at the end.</param>
    /// <param name="preserveSeparators">
    /// When true, characters that are not letters or digits are left visible and are
    /// not counted towards the visible lengths, so a card or phone number keeps its
    /// shape. When false, every character is maskable.
    /// </param>
    /// <returns>
    /// The masked string. Everything is masked when the visible lengths would leave
    /// nothing hidden, so a value shorter than expected is not revealed in full.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if either visible length is negative.</exception>
    /// <example>
    /// "1234-5678-9012-5678".Mask('*', 4, 4) returns "1234-****-****-5678"
    /// </example>
    public static string Mask(this string text, char maskChar = '*',
        int visiblePrefixLength = 0, int visibleSuffixLength = 0,
        bool preserveSeparators = true)
    {
        if (visiblePrefixLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(visiblePrefixLength), "Must be zero or greater.");
        }

        if (visibleSuffixLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(visibleSuffixLength), "Must be zero or greater.");
        }

        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var characters = text.ToCharArray();

        // Sized to the input rather than to the limit, so only what is needed
        // gets zero-initialized
        Span<int> maskableIndexes = characters.Length <= _stackAllocLimit
            ? stackalloc int[characters.Length]
            : new int[characters.Length];

        int maskableCount = 0;

        for (int i = 0; i < characters.Length; i++)
        {
            if (!preserveSeparators || char.IsLetterOrDigit(characters[i]))
            {
                maskableIndexes[maskableCount++] = i;
            }
        }

        // Leaving nothing masked would publish the whole value, so mask all of it
        bool maskEverything =
            visiblePrefixLength + visibleSuffixLength >= maskableCount;

        int firstMasked = maskEverything ? 0 : visiblePrefixLength;
        int lastMasked = maskEverything
            ? maskableCount - 1
            : maskableCount - visibleSuffixLength - 1;

        for (int i = firstMasked; i <= lastMasked; i++)
        {
            characters[maskableIndexes[i]] = maskChar;
        }

        return new string(characters);
    }

    /// <summary>
    /// Trims the text and reduces every run of whitespace inside it to a single space.
    /// </summary>
    /// <param name="text">
    /// String to collapse. A null returns null. A string of nothing but whitespace returns an
    /// empty string, because that is what trimming it leaves.
    /// </param>
    /// <returns>
    /// The collapsed string, or the same instance when there was nothing to change.
    /// </returns>
    /// <remarks>
    /// Any character <see cref="char.IsWhiteSpace(char)"/> accepts is collapsed, so tabs and line
    /// breaks go the same way as spaces, and every run is replaced by a single space rather than
    /// by the first character of the run.
    /// </remarks>
    /// <example>
    /// "  the   quick\tbrown\r\nfox  ".CollapseWhitespace() returns "the quick brown fox"
    /// </example>
    public static string CollapseWhitespace(this string text)
    {
        if (text == null)
        {
            return null;
        }

        var source = text.AsSpan();

        // Text that needs nothing done to it is the common case, and this scan is what
        // lets it return the caller's own instance and allocate nothing. It costs between
        // 2% and 8% on text that does need work, against 1.7x to 2x on text that does not.
        if (!NeedsWhitespaceCollapsed(source))
        {
            return text;
        }

        // Sized to the input rather than to the limit, so only what is needed
        // gets zero-initialized
        Span<char> collapsed = source.Length <= _stackAllocLimit
            ? stackalloc char[source.Length]
            : new char[source.Length];

        int count = 0;
        bool pendingSpace = false;

        for (int i = 0; i < source.Length; i++)
        {
            char character = source[i];

            if (char.IsWhiteSpace(character))
            {
                // The space is only written once something follows it, and never before
                // the first character, which trims both ends without a second pass
                pendingSpace = count > 0;

                continue;
            }

            if (pendingSpace)
            {
                collapsed[count++] = ' ';
                pendingSpace = false;
            }

            collapsed[count++] = character;
        }

        return new string(collapsed.Slice(0, count));
    }

    private static bool NeedsWhitespaceCollapsed(ReadOnlySpan<char> source)
    {
        if (source.Length == 0)
        {
            return false;
        }

        if (char.IsWhiteSpace(source[0]) ||
            char.IsWhiteSpace(source[source.Length - 1]))
        {
            return true;
        }

        // Both ends are known not to be whitespace by here, so the loop can skip them
        // and read i + 1 without a bounds check
        for (int i = 1; i < source.Length - 1; i++)
        {
            if (!char.IsWhiteSpace(source[i]))
            {
                continue;
            }

            // Anything but a lone space between two non-whitespace characters has to change
            if (source[i] != ' ' || char.IsWhiteSpace(source[i + 1]))
            {
                return true;
            }
        }

        return false;
    }
}