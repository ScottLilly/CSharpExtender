using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpExtender.Services;

/// <summary>
/// Checks a string against a list of regex patterns compiled into one.
/// </summary>
/// <remarks>
/// The patterns are combined into a single alternation, so a check is one pass of
/// one regex rather than a pass per pattern.
/// </remarks>
public class CompositeRegexMatcher
{
    // Null when there was nothing to match, which is how HasPatterns answers
    private readonly Regex? _combinedRegex;

    public CompositeRegexMatcher(IEnumerable<string>? patterns, bool ignoreCase = false)
    {
        // Materialized once. Left lazy, Where and Distinct would run again for
        // every pass over the sequence.
        var distinctPatterns =
            patterns?.Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList() ?? [];

        if (distinctPatterns.Count == 0)
        {
            _combinedRegex = null;

            return;
        }

        var combinedPattern = string.Join("|", distinctPatterns.Select(p => $"(?:{p})"));

        var options = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (ignoreCase)
        {
            options |= RegexOptions.IgnoreCase;
        }

        _combinedRegex = new Regex(combinedPattern, options, TimeSpan.FromSeconds(2));
    }

    /// <summary>
    /// Whether any pattern was supplied. False means nothing can ever match, so a
    /// caller can skip work it would only do to find that out.
    /// </summary>
    public bool HasPatterns => _combinedRegex != null;

    /// <summary>
    /// Whether the input matches any of the patterns. False when there are none.
    /// </summary>
    public bool MatchesAny(string input)
    {
        if (_combinedRegex == null)
        {
            return false; // Short-circuit for empty or all-empty patterns
        }

        return _combinedRegex.IsMatch(input);
    }

    /// <summary>
    /// Whether the input matches any of the patterns. False when there are none.
    /// </summary>
    /// <remarks>
    /// Takes a span so a caller building the text it is testing does not have to
    /// turn it into a string first.
    /// </remarks>
    public bool MatchesAny(ReadOnlySpan<char> input)
    {
        if (_combinedRegex == null)
        {
            return false;
        }

        return _combinedRegex.IsMatch(input);
    }
}
