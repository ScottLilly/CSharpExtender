using System.Collections.Generic;
using System.Linq;
using System;

namespace CSharpExtender.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        private static readonly List<string> s_lowerCaseWords =
            new List<string>()
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
        public static bool IsDigitsOnly(this string s) =>
            !s.Any(c => !char.IsDigit(c));

        /// <summary>
        /// Converts an IEnumerable of strings to a single string with line feeds between each string
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string ToStringWithLineFeeds(this IEnumerable<string> lines)
        {
            return string.Join("\r\n", lines);
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
}