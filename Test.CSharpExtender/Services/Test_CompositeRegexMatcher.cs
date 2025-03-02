using CSharpExtender.Services;

namespace Test.CSharpExtender.Services;

public class Test_CompositeRegexMatcher
{
    [Fact]
    public void MatchesAny_WithWildcards_ReturnsCorrectResults()
    {
        // Patterns:
        // 1) "^Hello\\s+World!?$" => anchored pattern, "Hello     World!" or "Hello  World"
        // 2) "Test(ing)? => matches "Test" or "Testing"
        // 3) "foo\\d{3}" => matches strings containing "foo" followed by exactly 3 digits

        var patterns = new[]
        {
            "^Hello\\s+World!?$",
            "Test(ing)?",
            "foo\\d{3}"
        };

        var matcher = new CompositeRegexMatcher(patterns, false);

        // Test strings that should match
        Assert.True(matcher.MatchesAny("Hello World"), "Should match '^Hello\\s+World$'");
        Assert.True(matcher.MatchesAny("Hello   World!"), "Should match '^Hello\\s+World!$'");
        Assert.True(matcher.MatchesAny("Test"), "Expect 'Test' to match 'Test(ing)?'");
        Assert.True(matcher.MatchesAny("Testing"), "Expect 'Testing' to match 'Test(ing)?'");
        Assert.True(matcher.MatchesAny("foo123"), "Should match 'foo\\d{3}'");

        // Test strings that should NOT match
        Assert.False(matcher.MatchesAny("HelloWorld"), "Missing space => doesn't match '^Hello\\s+World'");
        Assert.False(matcher.MatchesAny("foo12"), "Needs exactly 3 digits => 'foo\\d{3}' should not match 'foo12'");
    }

    [Fact]
    public void MatchesAny_WithCaseInsensitivePatterns()
    {
        // Patterns:
        // 1) "cat"
        // 2) "dog.*house"
        var patterns = new[] { "cat", "dog.*house" };

        // Initialize with ignoreCase=true
        var matcher = new CompositeRegexMatcher(patterns, true);

        // "CAT" -> should match "cat" if ignoring case
        Assert.True(matcher.MatchesAny("I have a CAT"));
        // "DogHouse", "DoG --- House", or any variation
        Assert.True(matcher.MatchesAny("DOGhouse"));
        Assert.True(matcher.MatchesAny("dog big house"));
        Assert.True(matcher.MatchesAny("My doG sMall House"));

        // Should not match something completely unrelated
        Assert.False(matcher.MatchesAny("elephant"));
    }

    [Theory]
    [InlineData("ABxxCD", true, true)]      // ignoring case: matches 'ab.*cd'
    [InlineData("ABxxCD", false, false)]    // case-sensitive => no match
    [InlineData("abXXcd", true, true)]      // ignoring case => match
    [InlineData("abXXcd", false, true)]     // exact match => 'ab.*cd' works
    [InlineData("HELLO WORLD!", true, true)]
    [InlineData("HELLO WORLD!", false, false)] // anchored pattern ^Hello\s+World!?$, mismatch if case-sensitive
    public void MatchesAny_CaseSensitivityComplex(string input, bool ignoreCase, bool expected)
    {
        var patterns = new[]
        {
            "ab.*cd",
            "^Hello\\s+World!?$"
        };

        var matcher = new CompositeRegexMatcher(patterns, ignoreCase);

        Assert.Equal(expected, matcher.MatchesAny(input));
    }

    [Fact]
    public void MatchesAny_LongInputWithWildcards_NoCatastrophicBacktracking()
    {
        // Test large input to ensure performance, especially with wildcard '.*'
        var patterns = new[] { "abc.*xyz" };
        var matcher = new CompositeRegexMatcher(patterns, false);

        // Create a large string of repeated characters not matching
        string longString = new string('a', 10000) + "the end";

        // Should be false because we don't have "abc" followed by "xyz" in that large string
        Assert.False(matcher.MatchesAny(longString));
    }
}
