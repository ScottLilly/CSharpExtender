﻿using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

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
        IEnumerable<string> lines = null;
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
}