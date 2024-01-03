namespace CSharpExtender.ExtensionMethods;

public static class StringExtensionMethods
{
    private static readonly List<string> s_lowerCaseWords =
        new()
        {
            "a",
            "an",
            "and",
            "as",
            "as long as",
            "at",
            "but",
            "by",
            "even if",
            "for",
            "from",
            "if",
            "if only",
            "in",
            "is",
            "into",
            "like",
            "near",
            "now that",
            "nor",
            "of",
            "off",
            "on",
            "on top of",
            "once",
            "onto",
            "or",
            "out of",
            "over",
            "past",
            "so",
            "so that",
            "than",
            "that",
            "the",
            "till",
            "to",
            "up",
            "upon",
            "with",
            "when",
            "yet"
        };

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
    /// Get string with the first character of each word in correct book title casing
    /// e.g. convert "a tale of two cities" to "A Tale of Two Cities"
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Returns a copy of this string,
    /// with the first character of each word in correct book title casing</returns>
    public static string ToTitleCase(this string text)
    {
        string cleanedText =
            text
                .Replace("-", " ")
                .Replace("_", " ");

        var rawWords =
            cleanedText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().UpperCaseFirstChar())
                .ToList();

        var properCasedWords = new List<string>();

        if (rawWords.Any())
        {
            // First word is always upper-case
            properCasedWords.Add(rawWords.First().UpperCaseFirstChar());

            // Find correct casing for subsequent words
            foreach (string word in rawWords.Skip(1))
            {
                var matchingLowerCaseWord =
                    s_lowerCaseWords.FirstOrDefault(lcw =>
                        word.Equals(lcw, StringComparison.InvariantCultureIgnoreCase));

                properCasedWords.Add(matchingLowerCaseWord ?? word.UpperCaseFirstChar());
            }
        }

        return string.Join(' ', properCasedWords);
    }

    public static bool HasText(this string? value) =>
        value != null && !string.IsNullOrWhiteSpace(value);

    public static bool DoesNotHaveText(this string? value) =>
        value == null || string.IsNullOrWhiteSpace(value);

    public static string? NullIfEmpty(this string? value) =>
    string.IsNullOrWhiteSpace(value) ? null : value;

    public static string? ToDigitsOnly(this string? value) =>
        string.IsNullOrWhiteSpace(value)
        ? null
        : new string(value.Where(char.IsDigit).ToArray());

    public static bool IsDigitsOnly(this string s) =>
        double.TryParse(s, out double i);

    public static string ToStringWithLineFeeds(this IEnumerable<string> lines)
    {
        return string.Join("\r\n", lines);
    }

    public static string Repeated(this string text, int times)
    {
        return string.Concat(Enumerable.Repeat(text, times));
    }

    public static bool IsNotNullEmptyOrWhitespace(this string? value) =>
    !string.IsNullOrWhiteSpace(value);

    public static bool IsNullEmptyOrWhitespace(this string? value) =>
        value == null || string.IsNullOrWhiteSpace(value);

    public static IEnumerable<string> SplitPath(this string path)
    {
        return path.Split('/', '\\');
    }

    public static bool IncludesTheWords(this string text, params string[] words)
    {
        if (string.IsNullOrWhiteSpace(text) ||
           words.Length == 0 ||
           words.All(string.IsNullOrWhiteSpace))
        {
            return false;
        }

        return words.All(word => text.Contains(word, StringComparison.CurrentCultureIgnoreCase));
    }

    public static string RemoveText(this string text, string textToRemove)
    {
        while (text.Contains(textToRemove, StringComparison.CurrentCultureIgnoreCase))
        {
            text = text.Replace(textToRemove, "", StringComparison.CurrentCultureIgnoreCase);
        }

        return text;
    }

    #region Private methods

    private static string UpperCaseFirstChar(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return new string(a);
    }

    #endregion
}