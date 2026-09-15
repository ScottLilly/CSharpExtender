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

    protected BaseRedactionService(IEnumerable<string> redactedPaths, bool ignoreCase = false)
    {
        _matcher = new CompositeRegexMatcher(redactedPaths, ignoreCase);
    }
}
