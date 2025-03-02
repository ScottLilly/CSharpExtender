using System.Text;
using CSharpExtender.ExtensionMethods;
using CSharpExtender.Common;
using CSharpExtender.Options;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_StringBuilderExtensionMethods
{
    [Fact]
    public void AppendIf_ConditionTrue_AppendsText()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions { ToUpper = true };

        sb.AppendIf(() => true, "test", options);

        Assert.Equal("TEST", sb.ToString());
    }

    [Fact]
    public void AppendIf_ConditionFalse_DoesNotAppend()
    {
        var sb = new StringBuilder();

        sb.AppendIf(() => false, "test");

        Assert.Empty(sb.ToString());
    }

    [Fact]
    public void AppendLineIf_ConditionTrue_AppendsLine()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions { PrefixText = "> " };

        sb.AppendLineIf(() => true, "test", options);

        Assert.Equal("> test" + Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void AppendLineIfNotEmpty_EmptyString_DoesNotAppend()
    {
        var sb = new StringBuilder();

        sb.AppendLineIfNotEmpty("");

        Assert.Empty(sb.ToString());
    }

    [Fact]
    public void AppendLineIfNotEmpty_NonEmptyString_AppendsWithOptions()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions
        {
            IndentLevel = 1,
            IndentDepth = 2,
            IndentType = IndentType.Spaces
        };

        sb.AppendLineIfNotEmpty("test", options);

        Assert.Equal("  test" + Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void AppendFormatted_WithOptions_FormatsCorrectly()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions
        {
            MaxLength = 5,
            SuffixText = "!"
        };

        sb.AppendFormatted("{0} {1}", options, "Hello", "World");

        Assert.Equal("Hello!", sb.ToString());
    }

    [Fact]
    public void AppendLineFormatted_WithHtmlEscape_EscapesCorrectly()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions { EscapeHtml = true };

        sb.AppendLineFormatted("{0}", options, "<p>");

        Assert.Equal("&lt;p&gt;" + Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void AppendJoined_WithItems_JoinsCorrectly()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions { ToLower = true };
        var items = new[] { "TEST", "CASE" };

        sb.AppendJoined(",", items, options);

        Assert.Equal("test,case", sb.ToString());
    }

    [Fact]
    public void AppendJoined_EmptyList_DoesNotAppend()
    {
        var sb = new StringBuilder();
        var items = new string[0];

        sb.AppendJoined(",", items);

        Assert.Empty(sb.ToString());
    }

    [Fact]
    public void AppendLineJoined_WithTabs_AppendsCorrectly()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions
        {
            IndentLevel = 2,
            IndentType = IndentType.Tabs
        };
        var items = new[] { "a", "b" };

        sb.AppendLineJoined(",", items, options);

        Assert.Equal("\t\ta,b" + Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void Append_BasicWithOptions_AppendsCorrectly()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions
        {
            Format = "[{0}]",
            PrefixText = "START",
            SuffixText = "END"
        };

        sb.Append("test", options);

        Assert.Equal("START[test]END", sb.ToString());
    }

    [Fact]
    public void AppendLine_WithAllOptions_AppendsCorrectly()
    {
        var sb = new StringBuilder();
        var options = new StringBuilderOptions
        {
            EscapeHtml = true,
            IndentLevel = 1,
            IndentType = IndentType.Spaces,
            IndentDepth = 3,
            MaxLength = 5,
            PrefixText = ">",
            SuffixText = "<",
            ToUpper = true,
            Format = "[{0}]"
        };

        sb.AppendLine("test & more", options);

        Assert.Equal("   >[TEST ]<" + Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void Append_NullOptions_WorksAsDefault()
    {
        var sb = new StringBuilder();

        sb.Append("test", null);

        Assert.Equal("test", sb.ToString());
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("test", "test")]
    public void Append_VariousInputs_HandlesCorrectly(string? input, string expected)
    {
        var sb = new StringBuilder();

        sb.Append(input);

        Assert.Equal(expected, sb.ToString());
    }
}