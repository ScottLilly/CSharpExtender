using CSharpExtender.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpExtender.Services;

public abstract class BaseRedactionService
{
    protected readonly Regex _redactedPathRegex;
    protected readonly bool _isEmptyPattern;

    public BaseRedactionService(IEnumerable<string> redactedPaths, bool ignoreCase = false)
    {
        redactedPaths = redactedPaths?.Where(p => !string.IsNullOrEmpty(p)).Distinct() ?? [];

        _isEmptyPattern = redactedPaths.None() || redactedPaths.All(string.IsNullOrEmpty);

        if (_isEmptyPattern)
        {
            _redactedPathRegex = null; // No regex needed
            return;
        }

        var combinedPattern = string.Join("|", redactedPaths.Select(p => $"(?:{p})"));

        var options = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (ignoreCase)
        {
            options |= RegexOptions.IgnoreCase;
        }

        _redactedPathRegex = new Regex(combinedPattern, options, TimeSpan.FromSeconds(2));
    }
}
