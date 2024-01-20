using CSharpExtender.ExtensionMethods;
using System.Text;

namespace Test.CSharpExtender.ExtensionMethods;

public class StringBuilderExtensionMethodsTests
{
    [Fact]
    public void AppendLineIfNotEmpty_ShouldNotAppend_WhenTextIsNull()
    {
        var sb = new StringBuilder();
        string text = null;

        sb.AppendLineIfNotEmpty(text);

        Assert.Equal("", sb.ToString());
    }

    [Fact]
    public void AppendLineIfNotEmpty_ShouldNotAppend_WhenTextIsEmpty()
    {
        var sb = new StringBuilder();
        string text = "";

        sb.AppendLineIfNotEmpty(text);

        Assert.Equal("", sb.ToString());
    }

    [Fact]
    public void AppendLineIfNotEmpty_ShouldNotAppend_WhenTextIsWhitespace()
    {
        var sb = new StringBuilder();
        string text = "   ";

        sb.AppendLineIfNotEmpty(text);

        Assert.Equal("", sb.ToString());
    }

    [Fact]
    public void AppendLineIfNotEmpty_ShouldAppend_WhenTextIsNotEmpty()
    {
        var sb = new StringBuilder();
        string text = "Hello, World!";

        sb.AppendLineIfNotEmpty(text);

        Assert.Equal("Hello, World!" + Environment.NewLine, sb.ToString());
    }
}