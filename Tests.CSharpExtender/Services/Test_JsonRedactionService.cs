using CSharpExtender.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace Tests.CSharpExtender.Services;

public class Test_JsonRedactionService
{
    [Fact]
    public void Redact_EmptyPattern_ReturnsOriginalObject()
    {
        // Arrange
        var service = new JsonRedactionService([]);
        var input = new JsonObject
        {
            ["name"] = "John",
            ["age"] = 30
        };
        var expected = input.ToJsonString();

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal(expected, result.ToJsonString());
    }

    [Fact]
    public void Redact_WithMatchingPath_RedactValue()
    {
        // Arrange
        var service = new JsonRedactionService(["name"]);
        var input = new JsonObject
        {
            ["name"] = "John",
            ["age"] = 30
        };

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result["name"]!.ToString());
        Assert.Equal(30, result["age"]!.GetValue<int>());
    }

    [Fact]
    public void RedactString_ValidJsonWithPath_RedactCorrectly()
    {
        // Arrange
        var service = new JsonRedactionService(["address.street"]);
        var input = @"{
            ""name"": ""John"",
            ""address"": {
                ""street"": ""123 Main St"",
                ""city"": ""Chicago""
            }
        }";

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result["address"]!["street"]!.ToString());
        Assert.Equal("Chicago", result["address"]!["city"]!.ToString());
    }

    [Fact]
    public void RedactString_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        var service = new JsonRedactionService([]);

        // Act & Assert
        Assert.Throws<JsonException>(() => service.Redact("invalid json"));
    }

    [Fact]
    public void RedactToString_ObjectEmptyPattern_ReturnsOriginal()
    {
        // Arrange
        var service = new JsonRedactionService([]);
        var input = new JsonObject
        {
            ["name"] = "Jane",
            ["age"] = 25
        };
        var expected = input.ToJsonString();

        // Act
        var result = service.RedactToString(input);

        // Normalize strings using JsonSerializer
        var normalizedResult = JsonSerializer.Serialize(JsonNode.Parse(result));

        // Assert
        Assert.Equal(expected, normalizedResult);
    }

    [Fact]
    public void RedactToString_ObjectWithArray_RedactArrayElements()
    {
        // Arrange
        var service = new JsonRedactionService(["items"]);
        var input = new JsonObject
        {
            ["items"] = new JsonArray("secret1", "secret2")
        };

        // Act
        var result = service.RedactToString(input);

        // Assert
        var parsed = JsonNode.Parse(result)!.AsObject();
        var items = parsed["items"]!;
        Assert.Null(items);
    }

    [Fact]
    public void RedactToString_StringWithNestedPath_RedactCorrectly()
    {
        // Arrange
        var service = new JsonRedactionService(["user.ssn"]);
        var input = @"{
            ""user"": {
                ""name"": ""Bob"",
                ""ssn"": ""123-45-6789""
            }
        }";

        // Act
        var result = service.RedactToString(input);

        // Assert
        var parsed = JsonNode.Parse(result)!.AsObject();
        Assert.Equal("Bob", parsed["user"]!["name"]!.ToString());
        Assert.Equal("", parsed["user"]!["ssn"]!.ToString());
    }

    [Fact]
    public void RedactToString_StringInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var service = new JsonRedactionService([]);

        // Act & Assert
        Assert.Throws<JsonException>(() => service.RedactToString("not json"));
    }

    [Fact]
    public void Redact_WithIgnoreCase_RedactCaseInsensitive()
    {
        // Arrange
        var service = new JsonRedactionService(["USERNAME"], true);
        var input = new JsonObject
        {
            ["username"] = "JohnDoe"
        };

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result["username"]!.ToString());
    }

    [Fact]
    public void Redact_WithDifferentTypes_RedactToDefaultValues()
    {
        // Arrange
        var service = new JsonRedactionService(["str", "num", "bool"]);
        var input = new JsonObject
        {
            ["str"] = "text",
            ["num"] = 42,
            ["bool"] = true
        };

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("", result["str"]!.ToString());
        Assert.Equal(0, result["num"]!.GetValue<int>());
        Assert.False(result["bool"]!.GetValue<bool>());
    }

    [Fact]
    public void Redact_NullNode_DoesNotThrow()
    {
        // Arrange
        var service = new JsonRedactionService(["data"]);
        var input = new JsonObject
        {
            ["data"] = null
        };

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Null(result["data"]);
    }

    [Fact]
    public void Redact_PatternMatchingRootPath_LeavesDocumentIntact()
    {
        // The root has no parent to write a replacement back into, so it is out
        // of scope. A pattern matching its empty path must not blank the document.

        // Arrange
        var service = new JsonRedactionService(["^$"]);
        var input = new JsonObject
        {
            ["secret"] = "value"
        };

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("value", result["secret"]!.ToString());
    }

    [Fact]
    public void Redact_PatternMatchingOneArrayIndex_RedactsOnlyThatElement()
    {
        // Pins the indexed part of the path, "people[1].ssn", which the walk builds
        // as it goes rather than holding as a string

        // Arrange
        var service = new JsonRedactionService([@"people\[1\]\.ssn"]);
        var input = new JsonObject
        {
            ["people"] = new JsonArray(
                new JsonObject { ["ssn"] = "first" },
                new JsonObject { ["ssn"] = "second" },
                new JsonObject { ["ssn"] = "third" })
        };

        // Act
        var result = service.Redact(input);

        // Assert
        Assert.Equal("first", result["people"]![0]!["ssn"]!.ToString());
        Assert.Equal("", result["people"]![1]!["ssn"]!.ToString());
        Assert.Equal("third", result["people"]![2]!["ssn"]!.ToString());
    }

    [Fact]
    public void Redact_PathLongerThanTheStartingBuffer_StillMatches()
    {
        // The path is built in a buffer that starts at 256 characters, so nesting
        // deep enough to outgrow it has to keep working

        // Arrange
        string segment = new string('a', 40);
        var service = new JsonRedactionService(["secret"]);

        var leaf = new JsonObject { ["secret"] = "hide me" };
        var node = leaf;

        for (int i = 0; i < 10; i++)
        {
            node = new JsonObject { [segment] = node };
        }

        // Act
        var result = service.Redact(node);

        // Assert
        JsonNode walked = result;

        for (int i = 0; i < 10; i++)
        {
            walked = walked[segment]!;
        }

        Assert.Equal("", walked["secret"]!.ToString());
    }

    [Fact]
    public void Redact_NullJsonObject_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new JsonRedactionService(["name"]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.Redact((JsonObject)null!));
    }

    [Fact]
    public void RedactToString_NullJsonObject_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new JsonRedactionService(["name"]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.RedactToString((JsonObject)null!));
    }

    [Fact]
    public void Redact_NullString_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new JsonRedactionService(["name"]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.Redact((string)null!));
    }

    [Fact]
    public void RedactToString_NullString_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new JsonRedactionService(["name"]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.RedactToString((string)null!));
    }

    [Fact]
    public void Redact_NullArgument_ThrowsTheSameAsTheXmlService()
    {
        // The two services expose the same members, so a caller switching between them has to
        // get the same answer from either one
        var json = new JsonRedactionService(["name"]);
        var xml = new XmlRedactionService(["name"]);

        Assert.Throws<ArgumentNullException>(() => json.Redact((JsonObject)null!));
        Assert.Throws<ArgumentNullException>(() => xml.Redact((XmlDocument)null!));

        Assert.Throws<ArgumentNullException>(() => json.RedactToString((JsonObject)null!));
        Assert.Throws<ArgumentNullException>(() => xml.RedactToString((XmlDocument)null!));
    }
}
