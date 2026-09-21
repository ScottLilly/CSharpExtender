using System.Xml;
using System.Xml.Linq;
using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

public class Test_XElementExtensionMethods
{
    private const string XML =
        "<person id=\"42\" active=\"true\" born=\"2026-09-15\" score=\"abc\" both=\"from the attribute\" " +
        "dotNetBool=\"True\" schemaBool=\"1\" schemaFalse=\"0\" utc=\"2026-09-15T10:30:00Z\" " +
        "dayFirst=\"15/09/2026\" grouped=\"1,234\">" +
        "<name>Scott</name>" +
        "<age>42</age>" +
        "<badAge>xyz</badAge>" +
        "<nested><inner>deep</inner></nested>" +
        "<tagged xmlns=\"http://example.com/ns\">namespaced</tagged>" +
        "<both>from the element</both>" +
        "</person>";

    private static XElement Root()
    {
        return XDocument.Parse(XML).Root!;
    }

    private static XmlNode XmlRoot()
    {
        var document = new XmlDocument();

        document.LoadXml(XML);

        return document.DocumentElement!;
    }

    [Fact]
    public void AttributeAsString_ReturnsTheValue()
    {
        Assert.Equal("42", Root().AttributeAsString("id"));
    }

    [Fact]
    public void AttributeAsString_MissingAttribute_ReturnsNull()
    {
        Assert.Null(Root().AttributeAsString("nope"));
    }

    [Fact]
    public void AttributeAsInt_ReturnsTheValue()
    {
        Assert.Equal(42, Root().AttributeAsInt("id"));
    }

    [Fact]
    public void AttributeAsInt_MissingOrUnparseable_ReturnsDefault()
    {
        Assert.Equal(0, Root().AttributeAsInt("nope"));
        Assert.Equal(0, Root().AttributeAsInt("score"));
    }

    [Fact]
    public void AttributeAsBool_ReturnsTheValue()
    {
        Assert.True(Root().AttributeAsBool("active"));
    }

    [Fact]
    public void AttributeAsBool_MissingOrUnparseable_ReturnsDefault()
    {
        Assert.False(Root().AttributeAsBool("nope"));
        Assert.False(Root().AttributeAsBool("score"));
    }

    [Fact]
    public void AttributeAsDateTime_ReturnsTheValue()
    {
        Assert.Equal(new DateTime(2026, 9, 15), Root().AttributeAsDateTime("born"));
    }

    [Fact]
    public void AttributeAsDateTime_MissingOrUnparseable_ReturnsDefault()
    {
        Assert.Equal(default, Root().AttributeAsDateTime("nope"));
        Assert.Equal(default, Root().AttributeAsDateTime("score"));
    }

    [Fact]
    public void ElementAsString_ReturnsTheText()
    {
        Assert.Equal("Scott", Root().ElementAsString("name"));
    }

    [Fact]
    public void ElementAsString_MissingElement_ReturnsNull()
    {
        Assert.Null(Root().ElementAsString("nope"));
    }

    [Fact]
    public void ElementAsString_ElementWithChildren_ReturnsTheConcatenatedText()
    {
        // XElement.Value and XmlNode.InnerText both flatten everything below the element
        Assert.Equal("deep", Root().ElementAsString("nested"));
    }

    [Fact]
    public void ElementAsString_NamespacedElement_IsNotMatchedByAnUnprefixedName()
    {
        // An unprefixed name matches only elements in no namespace, which is what the
        // XmlNode set does too
        Assert.Null(Root().ElementAsString("tagged"));
    }

    [Fact]
    public void ElementAsInt_ReturnsTheValue()
    {
        Assert.Equal(42, Root().ElementAsInt("age"));
    }

    [Fact]
    public void ElementAsInt_MissingOrUnparseable_ReturnsDefault()
    {
        Assert.Equal(0, Root().ElementAsInt("nope"));
        Assert.Equal(0, Root().ElementAsInt("badAge"));
    }

    [Fact]
    public void GetValue_AttributeWinsOverElementOfTheSameName()
    {
        Assert.Equal("from the attribute", Root().GetValue("both"));
    }

    [Fact]
    public void GetValue_NoAttribute_FallsBackToTheElement()
    {
        Assert.Equal("Scott", Root().GetValue("name"));
    }

    [Fact]
    public void GetValue_NeitherExists_ReturnsNull()
    {
        // Conformed to this set rather than to Pinax's copy, which returns ""
        Assert.Null(Root().GetValue("nope"));
    }

    [Fact]
    public void GetValue_FindsADirectChildRatherThanAnyDescendant()
    {
        // "inner" is a grandchild, so it is not found, which keeps GetValue looking in
        // the same places ElementAsString looks
        Assert.Null(Root().GetValue("inner"));
        Assert.Equal("deep", Root().ElementAsString("nested"));
    }

    [Fact]
    public void AttributeAsBool_AcceptsWhatDotNetWritesAndWhatXsBooleanWrites()
    {
        var element = Root();

        Assert.True(element.AttributeAsBool("dotNetBool"));
        Assert.True(element.AttributeAsBool("schemaBool"));
        Assert.False(element.AttributeAsBool("schemaFalse"));
    }

    [Fact]
    public void AttributeAsDateTime_UtcValue_KeepsItsKind()
    {
        var result = Root().AttributeAsDateTime("utc");

        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(new DateTime(2026, 9, 15, 10, 30, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void AttributeAsDateTime_DayFirstValue_ReturnsDefault()
    {
        Assert.Equal(default, Root().AttributeAsDateTime("dayFirst"));
    }

    [Fact]
    public void AttributeAsInt_GroupSeparators_ReturnDefault()
    {
        Assert.Equal(default, Root().AttributeAsInt("grouped"));
    }

    [Theory]
    [InlineData("id")]
    [InlineData("active")]
    [InlineData("born")]
    [InlineData("score")]
    [InlineData("nope")]
    [InlineData("dotNetBool")]
    [InlineData("schemaBool")]
    [InlineData("schemaFalse")]
    [InlineData("utc")]
    [InlineData("dayFirst")]
    [InlineData("grouped")]
    public void AttributeMethods_AnswerTheSameAsTheXmlNodeSet(string attributeName)
    {
        // The two sets look identical, so they have to agree. This is the check that
        // keeps them from drifting apart.
        var element = Root();
        var node = XmlRoot();

        Assert.Equal(node.AttributeAsString(attributeName),
            element.AttributeAsString(attributeName));
        Assert.Equal(node.AttributeAsInt(attributeName),
            element.AttributeAsInt(attributeName));
        Assert.Equal(node.AttributeAsBool(attributeName),
            element.AttributeAsBool(attributeName));
        Assert.Equal(node.AttributeAsDateTime(attributeName),
            element.AttributeAsDateTime(attributeName));
    }

    [Theory]
    [InlineData("name")]
    [InlineData("age")]
    [InlineData("badAge")]
    [InlineData("nested")]
    [InlineData("tagged")]
    [InlineData("nope")]
    public void ElementMethods_AnswerTheSameAsTheXmlNodeSet(string elementName)
    {
        var element = Root();
        var node = XmlRoot();

        Assert.Equal(node.ElementAsString(elementName),
            element.ElementAsString(elementName));
        Assert.Equal(node.ElementAsInt(elementName),
            element.ElementAsInt(elementName));
    }

    [Fact]
    public void AllMethods_NullElement_ThrowArgumentNullException()
    {
        XElement element = null!;

        Assert.Throws<ArgumentNullException>(() => element.AttributeAsString("id"));
        Assert.Throws<ArgumentNullException>(() => element.AttributeAsInt("id"));
        Assert.Throws<ArgumentNullException>(() => element.AttributeAsBool("id"));
        Assert.Throws<ArgumentNullException>(() => element.AttributeAsDateTime("id"));
        Assert.Throws<ArgumentNullException>(() => element.ElementAsString("name"));
        Assert.Throws<ArgumentNullException>(() => element.ElementAsInt("age"));
        Assert.Throws<ArgumentNullException>(() => element.GetValue("name"));
    }

    [Fact]
    public void AllMethods_NullName_ThrowArgumentNullException()
    {
        var element = Root();

        Assert.Throws<ArgumentNullException>(() => element.AttributeAsString(null!));
        Assert.Throws<ArgumentNullException>(() => element.AttributeAsInt(null!));
        Assert.Throws<ArgumentNullException>(() => element.AttributeAsBool(null!));
        Assert.Throws<ArgumentNullException>(() => element.AttributeAsDateTime(null!));
        Assert.Throws<ArgumentNullException>(() => element.ElementAsString(null!));
        Assert.Throws<ArgumentNullException>(() => element.ElementAsInt(null!));
        Assert.Throws<ArgumentNullException>(() => element.GetValue(null!));
    }
}
