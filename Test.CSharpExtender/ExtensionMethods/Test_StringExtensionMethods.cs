using CSharpExtender.ExtensionMethods;

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
    public void Test_ToTitleCase()
    {
        Assert.Equal("A Tale of Two Cities", "a tale of two cities".ToTitleCase());
        Assert.Equal("A Tale of Two Cities", "A Tale of Two Cities".ToTitleCase());
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
    public void Test_Repeat_Failure()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "a".Repeat(-1));
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