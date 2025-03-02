using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace CSharpExtender.Services;

public class CompositeRegexMatcher
{
    private readonly Regex _combinedRegex;
    private readonly bool _isEmptyPattern;

    public CompositeRegexMatcher(IEnumerable<string> patterns, bool ignoreCase = false)
    {
        patterns = patterns?.Where(p => !string.IsNullOrEmpty(p)).Distinct() ?? [];

        var combinedPattern = patterns.Any()
            ? string.Join("|", patterns.Select(p => $"(?:{p})"))
            : "";

        _isEmptyPattern = string.IsNullOrEmpty(combinedPattern);

        if (!_isEmptyPattern)
        {
            var options = RegexOptions.Compiled | RegexOptions.CultureInvariant;

            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }

            _combinedRegex = new Regex(combinedPattern, options, TimeSpan.FromSeconds(2));
        }
        else
        {
            _combinedRegex = null; // No regex needed
        }
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