using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using CSharpExtender.ExtensionMethods;

namespace CSharpExtender.Services;

public class CompositeRegexMatcher
{
    private readonly Regex _combinedRegex;
    private readonly bool _isEmptyPattern;

    public CompositeRegexMatcher(IEnumerable<string> patterns, bool ignoreCase = false)
    {
        patterns = patterns?.Where(p => !string.IsNullOrEmpty(p)).Distinct() ?? [];

        _isEmptyPattern = patterns.None() || patterns.All(string.IsNullOrEmpty);

        if (_isEmptyPattern)
        {
            _combinedRegex = null; // No regex needed
            return;
        }

        var combinedPattern = string.Join("|", patterns.Select(p => $"(?:{p})"));

        var options = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (ignoreCase)
        {
            options |= RegexOptions.IgnoreCase;
        }

        _combinedRegex = new Regex(combinedPattern, options, TimeSpan.FromSeconds(2));
    }

    public bool MatchesAny(string input)
    {
        if (_isEmptyPattern)
        {
            return false; // Short-circuit for empty or all-empty patterns
        }

        return _combinedRegex.IsMatch(input);
    }
}