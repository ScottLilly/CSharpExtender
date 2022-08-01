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
}