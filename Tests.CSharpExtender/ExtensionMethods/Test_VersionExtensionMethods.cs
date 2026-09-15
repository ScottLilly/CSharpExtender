using CSharpExtender.ExtensionMethods;
using CSharpExtender.Models;

namespace Tests.CSharpExtender.ExtensionMethods;

public class Test_VersionExtensionMethods
{
    [Theory]
    [InlineData("1.2.3", 1, 2, 3, 0, null, null)]
    [InlineData("1.2.3.4", 1, 2, 3, 4, null, null)]
    [InlineData("1.2", 1, 2, 0, 0, null, null)]
    [InlineData("7", 7, 0, 0, 0, null, null)]
    [InlineData("1.2.3-beta.1", 1, 2, 3, 0, "beta.1", null)]
    [InlineData("2.0.0-rc1", 2, 0, 0, 0, "rc1", null)]
    [InlineData("3.1.0+build7", 3, 1, 0, 0, null, "build7")]
    [InlineData("1.0.0-alpha.2+sha.abc123", 1, 0, 0, 0, "alpha.2", "sha.abc123")]
    [InlineData("1.2.3-x-y-z", 1, 2, 3, 0, "x-y-z", null)]
    public void ToSemanticVersion_ReadsEveryPart(string text, int major, int minor,
        int patch, int revision, string label, string metadata)
    {
        var version = text.ToSemanticVersion();

        Assert.Equal(major, version.Major);
        Assert.Equal(minor, version.Minor);
        Assert.Equal(patch, version.Patch);
        Assert.Equal(revision, version.Revision);
        Assert.Equal(label, version.PrereleaseLabel);
        Assert.Equal(metadata, version.BuildMetadata);
    }

    [Fact]
    public void ToSemanticVersion_AlphaTextIsReadRatherThanStripped()
    {
        // The bug not being copied forward: stripping everything that is not a digit or a
        // dot turns this into 1.2.34, which parses and is silently wrong
        var version = "1.2.3-beta4".ToSemanticVersion();

        Assert.Equal(3, version.Patch);
        Assert.Equal("beta4", version.PrereleaseLabel);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not a version")]
    [InlineData("1.2.3.4.5")]
    [InlineData("v1.2.3")]
    [InlineData("1..2")]
    [InlineData("1.2.3-")]
    [InlineData("1.2.3+")]
    [InlineData("-beta")]
    [InlineData("1.2.3-beta_1")]
    [InlineData("99999999999999999999")]
    public void ToSemanticVersion_UnreadableText_ThrowsFormatException(string text)
    {
        Assert.Throws<FormatException>(() => text!.ToSemanticVersion());
    }

    [Fact]
    public void ToSemanticVersion_Null_ThrowsArgumentNullException()
    {
        string? text = null;

        Assert.Throws<ArgumentNullException>(() => text!.ToSemanticVersion());
    }

    [Fact]
    public void ToSemanticVersion_SurroundingWhitespace_IsIgnored()
    {
        Assert.Equal(3, "  1.2.3  ".ToSemanticVersion().Patch);
    }

    [Theory]
    [InlineData("1.2.3", true)]
    [InlineData("1.2.3-beta.1", true)]
    [InlineData("not a version", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void TryParseSemanticVersion_ReportsWhetherItRead(string text, bool expected)
    {
        Assert.Equal(expected, text.TryParseSemanticVersion(out _));
    }

    [Fact]
    public void TryParseSemanticVersion_Failure_SetsNullRatherThanAZeroVersion()
    {
        // Pinax returned 0.0.0.0 from a swallowed catch, so a typo sailed through looking
        // like a valid version and stayed wrong forever
        Assert.False("not a version".TryParseSemanticVersion(out var version));
        Assert.Null(version);
    }

    [Theory]
    [InlineData("1.2.3-beta.1", "1.2.3")]
    [InlineData("1.2.3.4", "1.2.3.4")]
    [InlineData("1.2.3.0", "1.2.3")]
    [InlineData("3.1.0+build7", "3.1.0")]
    [InlineData("not a version", null)]
    public void NumericPartOfVersion_ReturnsTheNumbersOnly(string text, string expected)
    {
        Assert.Equal(expected, text.NumericPartOfVersion());
    }

    [Theory]
    [InlineData("1.2.3-beta.1", "beta.1")]
    [InlineData("2.0.0-rc1", "rc1")]
    [InlineData("1.2.3", null)]
    [InlineData("3.1.0+build7", null)]
    [InlineData("not a version", null)]
    public void PrereleaseLabelOf_ReturnsTheLabel(string text, string expected)
    {
        Assert.Equal(expected, text.PrereleaseLabelOf());
    }

    [Fact]
    public void IsPrerelease_SaysWhetherThereIsALabel()
    {
        Assert.True("1.2.3-beta.1".ToSemanticVersion().IsPrerelease);
        Assert.False("1.2.3".ToSemanticVersion().IsPrerelease);
        Assert.False("1.2.3+build7".ToSemanticVersion().IsPrerelease);
    }

    [Theory]
    [InlineData("1.2.3", "1.2.3")]
    [InlineData("1.2", "1.2.0")]
    [InlineData("1.2.3.0", "1.2.3")]
    [InlineData("1.2.3.4", "1.2.3.4")]
    [InlineData("1.2.3-beta.1", "1.2.3-beta.1")]
    [InlineData("1.0.0-alpha.2+sha.abc", "1.0.0-alpha.2+sha.abc")]
    public void ToString_RendersTheVersion(string text, string expected)
    {
        Assert.Equal(expected, text.ToSemanticVersion().ToString());
    }

    [Fact]
    public void ToVersion_DropsTheLabelAndMetadata()
    {
        var version = "1.2.3-beta.1+build7".ToSemanticVersion().ToVersion();

        Assert.Equal(new Version(1, 2, 3, 0), version);
    }

    #region Ordering

    [Theory]
    // Numeric parts, left to right
    [InlineData("1.0.0", "2.0.0")]
    [InlineData("2.0.0", "2.1.0")]
    [InlineData("2.1.0", "2.1.1")]
    [InlineData("2.1.1", "2.1.1.1")]
    // A prerelease sorts before the release it leads to
    [InlineData("1.0.0-alpha", "1.0.0")]
    // The example ladder from the SemVer spec
    [InlineData("1.0.0-alpha", "1.0.0-alpha.1")]
    [InlineData("1.0.0-alpha.1", "1.0.0-alpha.beta")]
    [InlineData("1.0.0-alpha.beta", "1.0.0-beta")]
    [InlineData("1.0.0-beta", "1.0.0-beta.2")]
    [InlineData("1.0.0-beta.2", "1.0.0-beta.11")]
    [InlineData("1.0.0-beta.11", "1.0.0-rc.1")]
    [InlineData("1.0.0-rc.1", "1.0.0")]
    public void CompareTo_OrdersByPrecedence(string lower, string higher)
    {
        var low = lower.ToSemanticVersion();
        var high = higher.ToSemanticVersion();

        Assert.True(low < high, $"{lower} should sort before {higher}");
        Assert.True(high > low);
        Assert.True(low <= high);
        Assert.True(high >= low);
    }

    [Fact]
    public void CompareTo_NumericLabelIdentifiers_CompareNumericallyNotAsText()
    {
        // As text "11" sorts before "2", which is the whole reason identifiers made only
        // of digits get their own rule
        var second = "1.0.0-beta.2".ToSemanticVersion();
        var eleventh = "1.0.0-beta.11".ToSemanticVersion();

        Assert.True(second < eleventh);
    }

    [Fact]
    public void CompareTo_HugeNumericIdentifiers_DoNotOverflow()
    {
        // Longer than any integer type, so the comparison cannot go through a parse
        var smaller = "1.0.0-build.99999999999999999999999".ToSemanticVersion();
        var larger = "1.0.0-build.99999999999999999999999999999".ToSemanticVersion();

        Assert.True(smaller < larger);
    }

    [Fact]
    public void CompareTo_LeadingZerosInANumericIdentifier_AreIgnored()
    {
        var padded = "1.0.0-beta.01".ToSemanticVersion();
        var plain = "1.0.0-beta.1".ToSemanticVersion();

        Assert.Equal(0, padded.CompareTo(plain));
    }

    [Fact]
    public void CompareTo_BuildMetadata_TakesNoPartInPrecedence()
    {
        var withA = "1.2.3+a".ToSemanticVersion();
        var withB = "1.2.3+b".ToSemanticVersion();

        Assert.Equal(0, withA.CompareTo(withB));
        Assert.False(withA < withB);
        Assert.False(withA > withB);
    }

    [Fact]
    public void Equality_CoversBuildMetadataEvenThoughPrecedenceDoesNot()
    {
        // The one case where equality and precedence disagree, which is inherent to the
        // SemVer rules rather than a choice made here
        var withA = "1.2.3+a".ToSemanticVersion();
        var withB = "1.2.3+b".ToSemanticVersion();

        Assert.NotEqual(withA, withB);
        Assert.Equal(0, withA.CompareTo(withB));
    }

    [Fact]
    public void Equality_TreatsAnAbsentFourthPartAsZero()
    {
        Assert.Equal("1.2.3".ToSemanticVersion(), "1.2.3.0".ToSemanticVersion());
    }

    [Fact]
    public void CompareTo_Null_SortsFirst()
    {
        var version = "1.2.3".ToSemanticVersion();

        Assert.Equal(1, version.CompareTo(null));
        Assert.True(version > null);
        Assert.True(null < version);
    }

    [Fact]
    public void Sorting_AListOfVersions_PutsThemInPrecedenceOrder()
    {
        var versions = new[]
        {
            "1.0.0", "1.0.0-rc.1", "2.0.0", "1.0.0-alpha", "1.0.1", "1.0.0-beta.11",
            "1.0.0-beta.2"
        }.Select(text => text.ToSemanticVersion()).ToList();

        versions.Sort();

        Assert.Equal(
            new[]
            {
                "1.0.0-alpha", "1.0.0-beta.2", "1.0.0-beta.11", "1.0.0-rc.1", "1.0.0",
                "1.0.1", "2.0.0"
            },
            versions.Select(version => version.ToString()));
    }

    [Fact]
    public void IComparable_NonVersionArgument_ThrowsArgumentException()
    {
        IComparable version = "1.2.3".ToSemanticVersion();

        Assert.Throws<ArgumentException>(() => version.CompareTo("1.2.3"));
    }

    #endregion
}
