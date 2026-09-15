using System;

namespace CSharpExtender.Models;

/// <summary>
/// A version number with its prerelease label and build metadata kept alongside the numeric
/// parts, so a value like "1.2.3-beta.1" survives as one thing.
/// </summary>
/// <remarks>
/// <see cref="Version"/> has no room for a label, which is why this exists: through a
/// <see cref="Version"/>, "1.2.3-beta.1" and "1.2.3" compare as equal, and a caller deciding
/// whether to upgrade gets the wrong answer with nothing to tell them so.
/// Ordering follows the Semantic Versioning precedence rules, so a prerelease sorts before
/// the release it leads to and build metadata is ignored. Equality is over every part,
/// build metadata included. Those two disagree in exactly one case: "1.2.3+a" and "1.2.3+b"
/// are not equal, but neither is greater than the other.
/// </remarks>
public sealed record SemanticVersion : IComparable<SemanticVersion>, IComparable
{
    /// <summary>The first numeric part.</summary>
    public int Major { get; init; }

    /// <summary>The second numeric part, or zero when the text did not carry one.</summary>
    public int Minor { get; init; }

    /// <summary>The third numeric part, or zero when the text did not carry one.</summary>
    public int Patch { get; init; }

    /// <summary>
    /// The fourth numeric part, or zero when the text did not carry one. Semantic Versioning
    /// has no fourth part; this is here because .NET version strings often do.
    /// </summary>
    public int Revision { get; init; }

    /// <summary>
    /// The prerelease label, without its leading "-", or null when there is none.
    /// "1.2.3-beta.1" gives "beta.1".
    /// </summary>
    public string PrereleaseLabel { get; init; }

    /// <summary>
    /// The build metadata, without its leading "+", or null when there is none.
    /// "3.1.0+build7" gives "build7".
    /// </summary>
    public string BuildMetadata { get; init; }

    /// <summary>
    /// Whether this is a prerelease, which is to say whether it carries a label.
    /// </summary>
    public bool IsPrerelease => PrereleaseLabel != null;

    /// <summary>
    /// The numeric parts on their own, with no label or metadata. "1.2.3-beta.1" gives
    /// "1.2.3".
    /// </summary>
    /// <remarks>
    /// The fourth part appears only when it is not zero, so "1.2.3" and "1.2.3.0" both give
    /// "1.2.3". An absent part and a zero part are the same value here.
    /// </remarks>
    public string NumericPart =>
        Revision == 0
            ? $"{Major}.{Minor}.{Patch}"
            : $"{Major}.{Minor}.{Patch}.{Revision}";

    /// <summary>
    /// Converts to a <see cref="Version"/>, for the APIs that take one.
    /// </summary>
    /// <returns>A Version holding the numeric parts.</returns>
    /// <remarks>
    /// Lossy on purpose, and the reason this type exists: the label and the metadata cannot
    /// be carried and are dropped. Two values differing only by a prerelease label convert
    /// to the same Version.
    /// </remarks>
    public Version ToVersion() => new Version(Major, Minor, Patch, Revision);

    /// <summary>
    /// Renders the version the way it would be written.
    /// </summary>
    /// <remarks>
    /// Not always the text it was read from: the fourth part is omitted when it is zero, so
    /// "1.2.3.0" comes back as "1.2.3", and "1.2" comes back as "1.2.0".
    /// </remarks>
    public override string ToString()
    {
        string label = PrereleaseLabel == null ? string.Empty : "-" + PrereleaseLabel;
        string metadata = BuildMetadata == null ? string.Empty : "+" + BuildMetadata;

        return NumericPart + label + metadata;
    }

    /// <summary>
    /// Compares by Semantic Versioning precedence.
    /// </summary>
    /// <param name="other">The version to compare against. A null sorts first.</param>
    /// <returns>Negative, zero or positive, the way IComparable asks for.</returns>
    /// <remarks>
    /// Numeric parts first, then the label. A version with no label outranks one with a
    /// label, so 1.2.3 is greater than 1.2.3-beta. Labels are compared identifier by
    /// identifier, splitting on ".": one made only of digits compares numerically and sorts
    /// before one that is not, and otherwise they compare by ASCII order. When every shared
    /// identifier matches, the label with more of them wins, so "beta" is less than
    /// "beta.1". Build metadata takes no part in this.
    /// </remarks>
    public int CompareTo(SemanticVersion other)
    {
        if (other is null)
        {
            return 1;
        }

        int numeric = Major.CompareTo(other.Major);

        if (numeric != 0)
        {
            return numeric;
        }

        numeric = Minor.CompareTo(other.Minor);

        if (numeric != 0)
        {
            return numeric;
        }

        numeric = Patch.CompareTo(other.Patch);

        if (numeric != 0)
        {
            return numeric;
        }

        numeric = Revision.CompareTo(other.Revision);

        if (numeric != 0)
        {
            return numeric;
        }

        return ComparePrereleaseLabels(PrereleaseLabel, other.PrereleaseLabel);
    }

    int IComparable.CompareTo(object obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is SemanticVersion other)
        {
            return CompareTo(other);
        }

        throw new ArgumentException(
            $"Object must be of type {nameof(SemanticVersion)}.", nameof(obj));
    }

    public static bool operator <(SemanticVersion left, SemanticVersion right) =>
        Compare(left, right) < 0;

    public static bool operator >(SemanticVersion left, SemanticVersion right) =>
        Compare(left, right) > 0;

    public static bool operator <=(SemanticVersion left, SemanticVersion right) =>
        Compare(left, right) <= 0;

    public static bool operator >=(SemanticVersion left, SemanticVersion right) =>
        Compare(left, right) >= 0;

    #region Private Methods

    private static int Compare(SemanticVersion left, SemanticVersion right) =>
        left is null ? (right is null ? 0 : -1) : left.CompareTo(right);

    private static int ComparePrereleaseLabels(string left, string right)
    {
        if (left == null)
        {
            // Neither carries a label, or this one is the release and outranks the other
            return right == null ? 0 : 1;
        }

        if (right == null)
        {
            return -1;
        }

        string[] leftIdentifiers = left.Split('.');
        string[] rightIdentifiers = right.Split('.');

        int shared = Math.Min(leftIdentifiers.Length, rightIdentifiers.Length);

        for (int i = 0; i < shared; i++)
        {
            int comparison = CompareIdentifiers(leftIdentifiers[i], rightIdentifiers[i]);

            if (comparison != 0)
            {
                return comparison;
            }
        }

        return leftIdentifiers.Length.CompareTo(rightIdentifiers.Length);
    }

    private static int CompareIdentifiers(string left, string right)
    {
        bool leftIsNumeric = IsAllDigits(left);
        bool rightIsNumeric = IsAllDigits(right);

        if (leftIsNumeric && rightIsNumeric)
        {
            // Compared by length and then by ASCII order rather than by parsing, because an
            // identifier can hold more digits than any integer type would take. Leading
            // zeros are dropped first, so "01" and "1" come out equal.
            ReadOnlySpan<char> leftDigits = WithoutLeadingZeros(left);
            ReadOnlySpan<char> rightDigits = WithoutLeadingZeros(right);

            return leftDigits.Length != rightDigits.Length
                ? leftDigits.Length.CompareTo(rightDigits.Length)
                : leftDigits.SequenceCompareTo(rightDigits);
        }

        // A numeric identifier always sorts before one that is not
        if (leftIsNumeric)
        {
            return -1;
        }

        if (rightIsNumeric)
        {
            return 1;
        }

        return string.CompareOrdinal(left, right);
    }

    private static bool IsAllDigits(string identifier)
    {
        if (identifier.Length == 0)
        {
            return false;
        }

        foreach (char character in identifier)
        {
            if (character < '0' || character > '9')
            {
                return false;
            }
        }

        return true;
    }

    private static ReadOnlySpan<char> WithoutLeadingZeros(string digits)
    {
        var trimmed = digits.AsSpan().TrimStart('0');

        // Every character was a zero, so the value is zero
        return trimmed.Length == 0 ? "0".AsSpan() : trimmed;
    }

    #endregion
}
