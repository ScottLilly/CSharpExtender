using CSharpExtender.Services;
using System.Xml;

namespace Tests.CSharpExtender.Services;

public class Test_XmlRedactionService
{
    [Fact]
    public void Redact_EmptyPattern_ReturnsOriginalDocument()
    {
        // Arrange
        var service = new XmlRedactionService([]);
        var input = LoadXml("<person><name>John</name><age>30</age></person>");
        var expected = input.OuterXml;

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal(expected, result.OuterXml);
    }

    [Fact]
    public void Redact_WithMatchingPath_RedactsValue()
    {
        // Arrange
        var service = new XmlRedactionService(["person.name"]);
        var input = LoadXml("<person><name>John</name><age>30</age></person>");

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result.SelectSingleNode("person/name")!.InnerText);
        Assert.Equal("30", result.SelectSingleNode("person/age")!.InnerText);
    }

    [Fact]
    public void Redact_NullDocument_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new XmlRedactionService(["person.name"]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.Redact((XmlDocument)null));
    }

    [Fact]
    public void RedactString_WithNestedPath_RedactsCorrectly()
    {
        // Arrange
        var service = new XmlRedactionService(["person.address.street"]);
        var input = "<person><name>John</name><address><street>123 Main St</street><city>Chicago</city></address></person>";

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result.SelectSingleNode("person/address/street")!.InnerText);
        Assert.Equal("Chicago", result.SelectSingleNode("person/address/city")!.InnerText);
        Assert.Equal("John", result.SelectSingleNode("person/name")!.InnerText);
    }

    [Fact]
    public void RedactString_InvalidXml_ThrowsXmlException()
    {
        // Arrange
        var service = new XmlRedactionService([]);

        // Act & Assert
        Assert.Throws<XmlException>(() => service.Redact("not xml"));
    }

    [Fact]
    public void Redact_MatchingParentElement_RemovesWholeSubtree()
    {
        // Arrange
        var service = new XmlRedactionService(["person.address"]);
        var input = LoadXml("<person><name>John</name><address><street>123 Main St</street></address></person>");

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Empty(result.SelectSingleNode("person/address")!.ChildNodes);
        Assert.Null(result.SelectSingleNode("person/address/street"));
        Assert.Equal("John", result.SelectSingleNode("person/name")!.InnerText);
    }

    [Fact]
    public void Redact_MatchingElement_KeepsAttributeNamesAndBlanksTheirValues()
    {
        // Arrange
        var service = new XmlRedactionService(["person.ssn"]);
        var input = LoadXml("""<person><ssn type="national">123-45-6789</ssn></person>""");

        // Act
        var result = service.Redact(input);

        // Assert
        var ssn = (XmlElement)result.SelectSingleNode("person/ssn")!;
        Assert.Equal("", ssn.InnerText);
        Assert.True(ssn.HasAttribute("type"));
        Assert.Equal("", ssn.GetAttribute("type"));
    }

    [Fact]
    public void Redact_MatchingAttributePath_RedactsAttributeValue()
    {
        // Arrange
        var service = new XmlRedactionService(["person.@id"]);
        var input = LoadXml("""<person id="12345"><name>John</name></person>""");

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result.DocumentElement!.GetAttribute("id"));
        Assert.Equal("John", result.SelectSingleNode("person/name")!.InnerText);
    }

    [Fact]
    public void Redact_AttributePattern_DoesNotRedactElementOfSameName()
    {
        // Arrange
        var service = new XmlRedactionService(["person.@name"]);
        var input = LoadXml("""<person name="attribute value"><name>element value</name></person>""");

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result.DocumentElement!.GetAttribute("name"));
        Assert.Equal("element value", result.SelectSingleNode("person/name")!.InnerText);
    }

    [Fact]
    public void Redact_RepeatedElements_RedactsEveryMatch()
    {
        // Arrange
        var service = new XmlRedactionService(["cart.item.price"]);
        var input = LoadXml("<cart><item><price>10</price></item><item><price>20</price></item></cart>");

        // Act
        var result = service.Redact(input);

        // Assert
        var prices = result.SelectNodes("cart/item/price")!;
        Assert.Equal(2, prices.Count);
        Assert.All(prices.Cast<XmlNode>(), p => Assert.Equal("", p.InnerText));
    }

    [Fact]
    public void Redact_WithIgnoreCase_RedactsCaseInsensitive()
    {
        // Arrange
        var service = new XmlRedactionService(["USER.PASSWORD"], true);
        var input = LoadXml("<user><password>hunter2</password></user>");

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result.SelectSingleNode("user/password")!.InnerText);
    }

    [Fact]
    public void Redact_WithoutIgnoreCase_DoesNotRedactDifferentCase()
    {
        // Arrange
        var service = new XmlRedactionService(["USER.PASSWORD"]);
        var input = LoadXml("<user><password>hunter2</password></user>");

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("hunter2", result.SelectSingleNode("user/password")!.InnerText);
    }

    [Fact]
    public void RedactToString_Document_ReturnsRedactedXml()
    {
        // Arrange
        var service = new XmlRedactionService(["user.password"]);
        var input = LoadXml("<user><name>Bob</name><password>hunter2</password></user>");

        // Act
        var result = service.RedactToString(input);

        // Assert
        Assert.Equal("<user><name>Bob</name><password /></user>", result);
    }

    [Fact]
    public void RedactToString_String_ReturnsRedactedXml()
    {
        // Arrange
        var service = new XmlRedactionService(["user.password"]);

        // Act
        var result = service.RedactToString("<user><name>Bob</name><password>hunter2</password></user>");

        // Assert
        Assert.Equal("<user><name>Bob</name><password /></user>", result);
    }

    [Fact]
    public void RedactToString_EmptyPattern_ReturnsOriginalXml()
    {
        // Arrange
        var service = new XmlRedactionService([]);
        var input = "<user><name>Bob</name><password>hunter2</password></user>";

        // Act
        var result = service.RedactToString(input);

        // Assert
        Assert.Equal(input, result);
    }

    private static XmlDocument LoadXml(string xml)
    {
        var document = new XmlDocument();
        document.LoadXml(xml);

        return document;
    }
}
