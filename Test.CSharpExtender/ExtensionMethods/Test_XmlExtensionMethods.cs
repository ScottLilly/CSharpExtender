using System.Xml;
using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_XmlExtensionMethods
{
    [Fact]
    public void AttributeAsInt_ValidAttribute_ReturnsParsedInt()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='123'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsInt("attr");

        Assert.Equal(123, result);
    }

    [Fact]
    public void AttributeAsInt_InvalidAttribute_ReturnsDefaultInt()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='abc'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsInt("attr");

        Assert.Equal(default, result);
    }

    [Fact]
    public void AttributeAsString_ValidAttribute_ReturnsString()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='abc'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsString("attr");

        Assert.Equal("abc", result);
    }

    [Fact]
    public void AttributeAsString_InvalidAttribute_ReturnsNull()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsString("attr");

        Assert.Null(result);
    }

    [Fact]
    public void AttributeAsBool_ValidAttribute_ReturnsParsedBool()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='true'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsBool("attr");

        Assert.True(result);
    }

    [Fact]
    public void AttributeAsBool_InvalidAttribute_ReturnsDefaultBool()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='abc'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsBool("attr");

        Assert.Equal(default, result);
    }

    [Fact]
    public void AttributeAsDateTime_ValidAttribute_ReturnsParsedDateTime()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='2022-01-01'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsDateTime("attr");

        Assert.Equal(new DateTime(2022, 1, 1), result);
    }

    [Fact]
    public void AttributeAsDateTime_InvalidAttribute_ReturnsDefaultDateTime()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root attr='abc'></root>");
        var node = doc.DocumentElement;

        var result = node.AttributeAsDateTime("attr");

        Assert.Equal(default, result);
    }

    [Fact]
    public void ElementAsString_ValidElement_ReturnsInnerText()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><child>abc</child></root>");
        var node = doc.DocumentElement;

        var result = node.ElementAsString("child");

        Assert.Equal("abc", result);
    }

    [Fact]
    public void ElementAsString_InvalidElement_ReturnsNull()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root></root>");
        var node = doc.DocumentElement;

        var result = node.ElementAsString("child");

        Assert.Null(result);
    }

    [Fact]
    public void ElementAsInt_ValidElement_ReturnsParsedInt()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><child>123</child></root>");
        var node = doc.DocumentElement;

        var result = node.ElementAsInt("child");

        Assert.Equal(123, result);
    }

    [Fact]
    public void ElementAsInt_InvalidElement_ReturnsDefaultInt()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><child>abc</child></root>");
        var node = doc.DocumentElement;

        var result = node.ElementAsInt("child");

        Assert.Equal(default, result);
    }

    [Fact]
    public void ElementAsString_XPathExpression_StillWorks()
    {
        // Anything that is not a plain element name goes to SelectSingleNode, so an
        // expression a caller already passes keeps behaving the way it always has
        var doc = new XmlDocument();
        doc.LoadXml("<root><contact><email>a@example.com</email></contact></root>");
        var node = doc.DocumentElement;

        Assert.Equal("a@example.com", node.ElementAsString("contact/email"));
        Assert.Equal("a@example.com", node.ElementAsString("*/email"));
    }

    [Fact]
    public void ElementAsString_NamespacedElement_IsNotFound()
    {
        // An XPath name with no prefix matches only elements in no namespace, and
        // the walk has to answer the same way
        var doc = new XmlDocument();
        doc.LoadXml("<root xmlns='urn:example'><child>value</child></root>");
        var node = doc.DocumentElement;

        Assert.Null(node.ElementAsString("child"));
    }

    [Fact]
    public void ElementAsString_NameWithHyphenOrDot_IsFound()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><first-name>Scott</first-name><a.b>value</a.b></root>");
        var node = doc.DocumentElement;

        Assert.Equal("Scott", node.ElementAsString("first-name"));
        Assert.Equal("value", node.ElementAsString("a.b"));
    }

    [Fact]
    public void ElementAsString_FindsTheFirstMatchingChildOnly()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><child>first</child><child>second</child></root>");
        var node = doc.DocumentElement;

        Assert.Equal("first", node.ElementAsString("child"));
    }

    [Fact]
    public void ElementAsString_MatchingNameDeeperDown_IsNotFound()
    {
        // The lookup is of child elements, not descendants, which is what an XPath
        // of a plain name means
        var doc = new XmlDocument();
        doc.LoadXml("<root><wrapper><child>value</child></wrapper></root>");
        var node = doc.DocumentElement;

        Assert.Null(node.ElementAsString("child"));
    }
}
