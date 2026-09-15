using CSharpExtender.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for reading version numbers out of strings.
/// </summary>
/// <remarks>
/// Version strings carry alpha text, and these are built to read it rather than strip it.
/// Stripping everything that is not a digit or a dot turns "1.2.3-beta4" into "1.2.34",
/// which parses without complaint and is silently wrong.
/// </remarks>
public static partial class VersionExtensionMethods
{
    // Semantic Versioning shaped, loosened to allow one to four numeric parts, because .NET
    // version strings often carry a fourth. [0-9] rather than \d, which in .NET also matches
    // the digits of other scripts.
    [GeneratedRegex(
        @"^(?<numeric>[0-9]+(\.[0-9]+){0,3})" +
        @"(-(?<prerelease>[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*))?" +
        @"(\+(?<metadata>[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*))?$")]
    private static partial Regex VersionPattern();

    /// <summary>
    /// Reads a version string into a <see cref="SemanticVersion"/>.
    /// </summary>
    /// <param name="text">The version string, such as "1.2.3-beta.1" or "3.1.0+build7".</param>
    /// <returns>The parsed version.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="text"/> is not a version string. Use
    /// <see cref="TryParseSemanticVersion"/> when unreadable input is expected rather than
    /// exceptional, the way Version.Parse and Version.TryParse divide the same work.
    /// </exception>
    public static SemanticVersion ToSemanticVersion(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (TryParseSemanticVersion(text, out var version))
        {
            return version;
        }

        throw new FormatException($"'{text}' is not a version string.");
    }

    /// <summary>
    /// Reads a version string into a <see cref="SemanticVersion"/>, without throwing.
    /// </summary>
    /// <param name="text">The version string. A null or unreadable value returns false.</param>
    /// <param name="version">The parsed version, or null when this returns false.</param>
    /// <returns>Whether the text was read.</returns>
    public static bool TryParseSemanticVersion(this string? text,
        [NotNullWhen(true)] out SemanticVersion? version)
    {
        version = null;

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        Match match = VersionPattern().Match(text.Trim());

        if (!match.Success)
        {
            return false;
        }

        string[] parts = match.Groups["numeric"].Value.Split('.');

        // Every part matched [0-9]+, so the only way the parse fails is a value too large
        // for an int, which is a version string nobody meant to write
        if (!TryReadPart(parts, 0, out int major) ||
            !TryReadPart(parts, 1, out int minor) ||
            !TryReadPart(parts, 2, out int patch) ||
            !TryReadPart(parts, 3, out int revision))
        {
            return false;
        }

        version = new SemanticVersion
        {
            Major = major,
            Minor = minor,
            Patch = patch,
            Revision = revision,
            PrereleaseLabel = GroupOrNull(match, "prerelease"),
            BuildMetadata = GroupOrNull(match, "metadata")
        };

        return true;
    }

    /// <summary>
    /// Returns the numeric part of a version string, with any label and metadata removed.
    /// </summary>
    /// <param name="text">The version string.</param>
    /// <returns>
    /// The numeric part, such as "1.2.3" from "1.2.3-beta.1", or null when the text is not a
    /// version string.
    /// </returns>
    public static string? NumericPartOfVersion(this string? text) =>
        text.TryParseSemanticVersion(out var version)
            ? version.NumericPart
            : null;

    /// <summary>
    /// Returns the prerelease label of a version string, without its leading "-".
    /// </summary>
    /// <param name="text">The version string.</param>
    /// <returns>
    /// The label, such as "beta.1" from "1.2.3-beta.1", or null when the version carries no
    /// label or the text is not a version string.
    /// </returns>
    public static string? PrereleaseLabelOf(this string? text) =>
        text.TryParseSemanticVersion(out var version)
            ? version.PrereleaseLabel
            : null;

    #region Private Methods

    private static bool TryReadPart(string[] parts, int index, out int value)
    {
        if (index >= parts.Length)
        {
            value = 0;

            return true;
        }

        return int.TryParse(parts[index], NumberStyles.None,
            CultureInfo.InvariantCulture, out value);
    }

    private static string? GroupOrNull(Match match, string groupName)
    {
        Group group = match.Groups[groupName];

        return group.Success ? group.Value : null;
    }

    #endregion
}
