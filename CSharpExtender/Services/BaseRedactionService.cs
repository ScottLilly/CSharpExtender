using System.Collections.Generic;

namespace CSharpExtender.Services;

/// <summary>
/// Shared base for the redaction services: holds the patterns that decide which
/// paths get redacted.
/// </summary>
/// <remarks>
/// The matching itself is <see cref="CompositeRegexMatcher"/>'s job.
/// </remarks>
public abstract class BaseRedactionService
{
    /// <summary>
    /// The paths to redact, combined into one matcher.
    /// </summary>
    protected readonly CompositeRegexMatcher _matcher;

    /// <summary>
    /// Instance constructor.
    /// </summary>
    /// <param name="redactedPaths">Regex patterns for the paths whose values are redacted.</param>
    /// <param name="ignoreCase">Match the patterns without regard to case.</param>
    protected BaseRedactionService(IEnumerable<string> redactedPaths, bool ignoreCase = false)
    {
        _matcher = new CompositeRegexMatcher(redactedPaths, ignoreCase);
    }
}
