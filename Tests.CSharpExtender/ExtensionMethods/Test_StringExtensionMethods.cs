using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

public class Test_StringExtensionMethods
{
    [Fact]
    public void Test_Matches()
    {
        Assert.True("asd".Matches("AsD"));
        Assert.False("asde".Matches("ASD"));
        Assert.False("".Matches("a"));
        Assert.False("a".Matches(""));
    }

    [Fact]
    public void Test_DoesNotMatch()
    {
        Assert.False("asd".DoesNotMatch("AsD"));
        Assert.True("asde".DoesNotMatch("ASD"));
        Assert.True("".DoesNotMatch("a"));
        Assert.True("a".DoesNotMatch(""));
    }

    [Fact]
    public void HasText_WithNonEmptyString_ReturnsTrue()
    {
        Assert.True("Hello, World!".HasText());
    }

    [Fact]
    public void HasText_WithEmptyString_ReturnsFalse()
    {
        Assert.False(string.Empty.HasText());
    }

    [Fact]
    public void HasText_WithWhitespaceString_ReturnsFalse()
    {
        Assert.False("   ".HasText());
    }

    [Fact]
    public void DoesNotHaveText_WithNonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, World!".DoesNotHaveText());
    }

    [Fact]
    public void DoesNotHaveText_WithEmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.DoesNotHaveText());
    }

    [Fact]
    public void DoesNotHaveText_WithWhitespaceString_ReturnsTrue()
    {
        Assert.True("   ".DoesNotHaveText());
    }

    [Fact]
    public void NullIfEmpty_WithNonEmptyString_ReturnsOriginalString()
    {
        string value = "Hello, World!";
        string result = value.NullIfEmpty();
        Assert.Equal(value, result);
    }

    [Fact]
    public void NullIfEmpty_WithEmptyString_ReturnsNull()
    {
        string value = string.Empty;
        string result = value.NullIfEmpty();
        Assert.Null(result);
    }

    [Fact]
    public void NullIfEmpty_WithWhitespaceString_ReturnsNull()
    {
        string value = "   ";
        string result = value.NullIfEmpty();
        Assert.Null(result);
    }

    [Fact]
    public void ToDigitsOnly_WithNonEmptyStringContainingDigits_ReturnsDigitsOnly()
    {
        string value = "abc123xyz456";
        string result = value.ToDigitsOnly();
        Assert.Equal("123456", result);
    }

    [Fact]
    public void ToDigitsOnly_WithEmptyString_ReturnsNull()
    {
        string value = string.Empty;
        string result = value.ToDigitsOnly();
        Assert.Null(result);
    }

    [Fact]
    public void ToDigitsOnly_WithWhitespaceString_ReturnsNull()
    {
        string value = "   ";
        string result = value.ToDigitsOnly();
        Assert.Null(result);
    }

    [Fact]
    public void ToDigitsOnly_WithNonEmptyStringWithoutDigits_ReturnsEmptyString()
    {
        string value = "abcXYZ";
        string result = value.ToDigitsOnly();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Test_IsDigitsOnly()
    {
        Assert.True("123".IsDigitsOnly());
        Assert.False("123a".IsDigitsOnly());
    }

    [Fact]
    public void IsDigitsOnly_NullString_ReturnsFalse()
    {
        Assert.False(((string)null).IsDigitsOnly());
    }

    [Fact]
    public void IsDigitsOnly_EmptyString_ReturnsTrue()
    {
        Assert.True("".IsDigitsOnly());
    }

    [Fact]
    public void Test_Repeat_Success()
    {
        Assert.Equal("aaa", "a".Repeat(3));
        Assert.Equal("", "a".Repeat(0));
    }


    [Fact]
    public void ToStringWithLineFeeds_WithValidEnumerable_ReturnsJoinedStringWithLineFeeds()
    {
        IEnumerable<string> lines = new List<string> { "Line 1", "Line 2", "Line 3" };
        string result = lines.ToStringWithLineFeeds();
        Assert.Equal($"Line 1{Environment.NewLine}Line 2{Environment.NewLine}Line 3", result);
    }

    [Fact]
    public void ToStringWithLineFeeds_WithEmptyEnumerable_ReturnsEmptyString()
    {
        IEnumerable<string> lines = new List<string>();
        string result = lines.ToStringWithLineFeeds();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToStringWithLineFeeds_WithNullEnumerable_ThrowsArgumentNullException()
    {
        IEnumerable<string>? lines = null;
        Assert.Throws<ArgumentNullException>(lines.ToStringWithLineFeeds);
    }

    [Fact]
    public void Test_Repeat_Failure()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "a".Repeat(-1));
    }

    [Fact]
    public void SplitPath_WithValidPathSingleForwardSlash_ReturnsSplitPath()
    {
        string path = "folder1/folder2/folder3";
        IEnumerable<string> result = path.SplitPath();
        Assert.Equal(new[] { "folder1", "folder2", "folder3" }, result);
    }

    [Fact]
    public void SplitPath_WithValidPathDoubleBackslash_ReturnsSplitPath()
    {
        string path = @"folder1\\folder2\\folder3";
        IEnumerable<string> result = path.SplitPath();
        Assert.Equal(new[] { "folder1", "folder2", "folder3" }, result);
    }

    [Fact]
    public void SplitPath_WithEmptyPath_ReturnsEmptyEnumerable()
    {
        string path = string.Empty;
        IEnumerable<string> result = path.SplitPath();
        Assert.Empty(result);
    }

    [Fact]
    public void SplitPath_WithWhitespaceOnlySegment_DoesNotReturnAnEmptyEntry()
    {
        Assert.Equal(new[] { "a", "b" }, @"a/ /b".SplitPath());
        Assert.Equal(new[] { "a", "b" }, @"a\   \b".SplitPath());
    }

    [Fact]
    public void SplitPath_WithWhitespaceOnlyPath_ReturnsEmptyEnumerable()
    {
        Assert.Empty("   ".SplitPath());
        Assert.Empty(" / / ".SplitPath());
    }

    [Fact]
    public void SplitPath_WithPaddedSegments_TrimsThem()
    {
        Assert.Equal(new[] { "home", "user", "projects" },
            @" home / user \ projects ".SplitPath());
    }

    [Fact]
    public void SplitPath_WithConsecutiveSeparators_DropsTheEmptySegments()
    {
        Assert.Equal(new[] { "a", "b" }, @"a//b".SplitPath());
        Assert.Equal(new[] { "a", "b" }, @"/a/b/".SplitPath());
    }

    [Fact]
    public void IncludesTheWords_WithValidTextAndWords_ReturnsTrue()
    {
        string text = "This is a sample text";
        string[] requiredWords = { "sample", "Text" };
        bool result = text.IncludesTheWords(requiredWords);
        Assert.True(result);
    }

    [Fact]
    public void IncludesTheWords_WithEmptyText_ReturnsFalse()
    {
        string text = string.Empty;
        string[] requiredWords = { "sample", "Text" };
        bool result = text.IncludesTheWords(requiredWords);
        Assert.False(result);
    }

    [Fact]
    public void IncludesTheWords_WithEmptyRequiredWords_ReturnsFalse()
    {
        string text = "This is a sample text";
        string[] requiredWords = Array.Empty<string>();
        bool result = text.IncludesTheWords(requiredWords);
        Assert.False(result);
    }

    [Fact]
    public void IncludesTheWords_WithNullOrWhiteSpaceRequiredWords_ReturnsFalse()
    {
        string text = "This is a sample text";
        string[] requiredWords = { " ", "  " };
        bool result = text.IncludesTheWords(requiredWords);
        Assert.False(result);
    }

    [Fact]
    public void IncludesTheWords_WithOrdinal_IsCaseSensitive()
    {
        string text = "This is a sample text";

        Assert.True(text.IncludesTheWords(StringComparison.Ordinal, "sample", "text"));
        Assert.False(text.IncludesTheWords(StringComparison.Ordinal, "sample", "Text"));
    }

    [Fact]
    public void IncludesTheWords_WithOrdinalIgnoreCase_IsNotCaseSensitive()
    {
        string text = "This is a sample text";

        Assert.True(text.IncludesTheWords(StringComparison.OrdinalIgnoreCase, "sample", "Text"));
        Assert.False(text.IncludesTheWords(StringComparison.OrdinalIgnoreCase, "sample", "missing"));
    }

    [Fact]
    public void IncludesTheWords_WithoutAComparison_StillIgnoresCase()
    {
        // The overload taking no comparison has to keep the original behavior
        string text = "This is a sample text";

        Assert.True(text.IncludesTheWords("sample", "Text"));
    }

    [Fact]
    public void IncludesTheWords_WithComparison_KeepsTheGuardClauseAnswers()
    {
        Assert.False("".IncludesTheWords(StringComparison.Ordinal, "sample"));
        Assert.False("This is a sample text".IncludesTheWords(StringComparison.Ordinal));
        Assert.False("This is a sample text"
            .IncludesTheWords(StringComparison.Ordinal, " ", "  "));
    }


    [Fact]
    public void Test_RemoveText()
    {
        Assert.Equal("a", "a".RemoveText(""));
        Assert.Equal("", "a".RemoveText("a"));
        Assert.Equal("a", "a".RemoveText("b"));
        Assert.Equal("a", "ab".RemoveText("b"));
        Assert.Equal("a", "ba".RemoveText("b"));
        Assert.Equal("a", "bab".RemoveText("b"));
        Assert.Equal("AA", "BABA".RemoveText("b"));
        Assert.Equal("BABA", "BABA".RemoveText("b", StringComparison.CurrentCulture));
    }

    [Fact]
    public void RemoveText_RemovalThatCreatesANewMatch_KeepsGoing()
    {
        // Taking "ab" out of "aabb" leaves "ab", which has to go too
        Assert.Equal("", "aabb".RemoveText("ab"));
        Assert.Equal("", "aaabbb".RemoveText("ab"));
        Assert.Equal("c", "aabbc".RemoveText("ab"));
        Assert.Equal("aacbb", "aacbb".RemoveText("ab"));
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
    public void RemoveText_NoMatch_ReturnsTheInputForEveryComparison(StringComparison comparison)
    {
        // The loop ends on an unchanged length, so a comparison that never matches
        // has to terminate rather than spin
        Assert.Equal("the quick brown fox", "the quick brown fox".RemoveText("zzz", comparison));
    }

    [Fact]
    public void Test_Repeat_NullOrEmptyText_ReturnsEmptyString()
    {
        Assert.Equal("", ((string)null).Repeat(3));
        Assert.Equal("", "".Repeat(3));
        Assert.Equal("", ((string)null).Repeat(0));
    }

    [Fact]
    public void Test_Repeat_MultiCharacterText()
    {
        Assert.Equal("abcabcabc", "abc".Repeat(3));
        Assert.Equal("abc", "abc".Repeat(1));
        Assert.Equal(300, "abc".Repeat(100).Length);
    }

    [Fact]
    public void ToDigitsOnly_LongStringPastTheStackBuffer_ReturnsAllDigits()
    {
        // 256 characters is where the implementation switches from a stack buffer
        // to a heap array, so cover both sides of it
        foreach (int length in new[] { 255, 256, 257, 1000 })
        {
            string value = string.Concat(Enumerable.Repeat("a1", length / 2));

            string result = value.ToDigitsOnly();

            Assert.Equal(length / 2, result.Length);
            Assert.True(result.IsDigitsOnly());
        }
    }

    [Fact]
    public void ToDigitsOnly_StringOfNonDigits_ReturnsEmptyString()
    {
        Assert.Equal("", "abcXYZ".ToDigitsOnly());
    }

    [Fact]
    public void ConvertFromString_Int_Success()
    {
        string input = "123";

        int result = input.ConvertFromString<int>();

        Assert.Equal(123, result);
    }

    [Fact]
    public void ConvertFromString_Bool_Success()
    {
        string input = "true";

        bool result = input.ConvertFromString<bool>();

        Assert.True(result);
    }

    [Fact]
    public void ConvertFromString_DateTime_Success()
    {
        string input = "2024-01-20";

        DateTime result = input.ConvertFromString<DateTime>();

        Assert.Equal(new DateTime(2024, 1, 20), result);
    }

    [Fact]
    public void ConvertFromString_UnsupportedType_ThrowsNotSupportedException()
    {
        string input = "example";

        Assert.Throws<NotSupportedException>(input.ConvertFromString<MyUnsupportedType>);
    }

    [Fact]
    public void ConvertFromString_NullInput_ThrowsException()
    {
        string? input = null;

        Assert.Throws<NotSupportedException>(() => input.ConvertFromString<int>());
    }

    [Fact]
    public void ConvertFromString_NullInput_IsNullable()
    {
        string? input = null;

        Assert.Null(input.ConvertFromString<int?>());
    }

    [Fact]
    public void PascalCaseStringSplit()
    {
        var output = "asd".SplitPascalCase();
        Assert.Single(output);
        Assert.Equal("asd", output.First());

        output = "asdAsd".SplitPascalCase();
        Assert.Equal(2, output.Count);
        Assert.Equal("asd", output.First());
        Assert.Equal("Asd", output.Last());

        output = "asdAsdQwe".SplitPascalCase();
        Assert.Equal(3, output.Count);
        Assert.Equal("asd", output.First());
        Assert.Equal("Asd", output[1]);
        Assert.Equal("Qwe", output.Last());
    }

    [Fact]
    public void SplitPascalCase_NullString_ReturnsEmptyList()
    {
        Assert.Empty(((string)null).SplitPascalCase());
    }

    [Fact]
    public void SplitPascalCase_EmptyString_ReturnsEmptyList()
    {
        Assert.Empty("".SplitPascalCase());
    }

    [Theory]
    // A capital at the start is not a boundary
    [InlineData("Asd", new[] { "Asd" })]
    // Nor is one following an underscore or any other non-word character
    [InlineData("asd_Asd", new[] { "asd_Asd" })]
    [InlineData("asd-Asd", new[] { "asd-Asd" })]
    // A run of capitals splits at each one
    [InlineData("XMLHttpRequest", new[] { "X", "M", "L", "Http", "Request" })]
    // A digit before a capital is a boundary
    [InlineData("Address1Line2", new[] { "Address1", "Line2" })]
    // Text that already has spaces splits on those too
    [InlineData("first Second", new[] { "first", "Second" })]
    public void SplitPascalCase_EdgeCases_SplitWhereTheyAlwaysHave(
        string input, string[] expected)
    {
        Assert.Equal(expected, input.SplitPascalCase());
    }

    [Fact]
    public void IncludesTheWords_MatchesSubstringsRatherThanWholeWords()
    {
        // Each word is looked for as a substring, so punctuation around it in the
        // text does not stop a match, and a word inside a longer one counts
        Assert.True("The dog, and the cat.".IncludesTheWords("dog", "cat"));
        Assert.True("The dog, and the cat.".IncludesTheWords("dog,", "cat."));
        Assert.True("concatenate".IncludesTheWords("cat"));
        Assert.True("first-class".IncludesTheWords("t-c"));
    }

    [Fact]
    public void TrimStringToMaximumLength()
    {
        Assert.Equal("asd", "asdfgh".ToMaxLengthOf(3));
        Assert.Equal("", "asdfgh".ToMaxLengthOf(0));

        Assert.Throws<ArgumentOutOfRangeException>(() => "asd".ToMaxLengthOf(-1));
    }

    [Fact]
    public void ToMaxLengthOf_NegativeLength_NamesTheParameterInTheException()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => "asd".ToMaxLengthOf(-1));

        Assert.Equal("maxLength", exception.ParamName);
        Assert.Contains("Must be zero or greater.", exception.Message);
    }

    [Theory]
    [InlineData("the quick brown fox", "the quick brown fox")]
    [InlineData("  the quick brown fox  ", "the quick brown fox")]
    [InlineData("the   quick    brown fox", "the quick brown fox")]
    [InlineData("  the   quick\tbrown\r\n fox  ", "the quick brown fox")]
    [InlineData("\t\r\n the \t quick \r\n", "the quick")]
    [InlineData("one", "one")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("\t\r\n", "")]
    [InlineData(" a ", "a")]
    [InlineData("a  b", "a b")]
    public void CollapseWhitespace_TrimsAndCollapsesRuns(string text, string expected)
    {
        Assert.Equal(expected, text.CollapseWhitespace());
    }

    [Fact]
    public void CollapseWhitespace_Null_ReturnsNull()
    {
        string text = null;

        Assert.Null(text.CollapseWhitespace());
    }

    [Fact]
    public void CollapseWhitespace_NothingToChange_ReturnsTheSameInstance()
    {
        // The scan in front of the collapse exists to make this true, so it is worth
        // asserting rather than just checking the value
        var text = new string("the quick brown fox".ToCharArray());

        Assert.Same(text, text.CollapseWhitespace());
    }

    [Fact]
    public void CollapseWhitespace_CollapsesNonSpaceWhitespaceRunToASingleSpace()
    {
        // A run is replaced by a space, not by the first character of the run
        Assert.Equal("a b", "a\t\t\tb".CollapseWhitespace());
        Assert.Equal("a b", "a\r\nb".CollapseWhitespace());
        Assert.Equal("a b", "a\tb".CollapseWhitespace());
    }

    [Fact]
    public void CollapseWhitespace_LongerThanTheStackBuffer_CollapsesTheSame()
    {
        // Past 256 characters the method builds on the heap instead of the stack
        var text = string.Concat(Enumerable.Repeat("word   ", 100));

        var expected = string.Join(" ", Enumerable.Repeat("word", 100));

        Assert.Equal(expected, text.CollapseWhitespace());
    }

    // Define a custom type that does not support string conversion for testing
    private class MyUnsupportedType
    {
    }
}