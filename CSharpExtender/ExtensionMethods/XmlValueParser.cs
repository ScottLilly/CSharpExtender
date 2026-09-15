using System;
using System.Globalization;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// The parsing rules shared by <see cref="XmlExtensionMethods"/> and
/// <see cref="XElementExtensionMethods"/>.
/// </summary>
/// <remarks>
/// Shared rather than copied into each class, because the two sets are meant to answer
/// identically and a copy is free to drift.
/// Every rule is culture-independent. A document does not change meaning when it crosses a
/// border, so nothing here may depend on the machine's locale. They keep the TryParse shape
/// rather than using XmlConvert, which has no Try variants: these methods return the type's
/// default for a value that will not parse, and catching an exception to do that would make
/// the normal path for malformed input an exception.
/// </remarks>
internal static class XmlValueParser
{
    internal static bool TryParseInt(string text, out int result) =>
        int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

    /// <summary>
    /// Parses the union of what .NET and xs:boolean write, rather than either alone.
    /// </summary>
    /// <remarks>
    /// bool.ToString writes "True", which xs:boolean rejects for its casing. Schema-generated
    /// XML writes "1" or "0", which bool.TryParse rejects outright. XML in the wild carries
    /// both, so accepting only one of them silently reads half of the valid input as false.
    /// </remarks>
    internal static bool TryParseBoolean(string text, out bool result)
    {
        ReadOnlySpan<char> trimmed = text.AsSpan().Trim();

        if (trimmed.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            trimmed.Equals("1", StringComparison.Ordinal))
        {
            result = true;

            return true;
        }

        if (trimmed.Equals("false", StringComparison.OrdinalIgnoreCase) ||
            trimmed.Equals("0", StringComparison.Ordinal))
        {
            result = false;

            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    /// Parses a date under the invariant culture, keeping whatever the text said about its
    /// time zone.
    /// </summary>
    /// <remarks>
    /// RoundtripKind is what stops a "Z" value being converted to the machine's local time and
    /// losing the fact that it was UTC. A value with no zone stays Unspecified rather than
    /// being assumed to be anything. A value with an explicit offset comes back as Local: the
    /// instant is right, but the clock reading is the machine's. AdjustToUniversal would
    /// normalize that one, at the cost of assuming an unzoned value is local and converting it
    /// too, which would put the machine's locale back into the answer.
    /// </remarks>
    internal static bool TryParseDateTime(string text, out DateTime result) =>
        DateTime.TryParse(text, CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out result);
}
