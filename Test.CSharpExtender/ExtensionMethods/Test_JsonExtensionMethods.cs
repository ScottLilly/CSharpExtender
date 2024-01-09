using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_JsonExtensionMethods
{
    [Fact]
    public void GetValueFromJsonPath_ValidPath_ReturnsValue()
    {
        string json = "{\"name\":\"John\", \"age\":30}";
        string path = "name";

        string result = json.GetValueFromJsonPath(path);

        Assert.Equal("John", result);
    }

    [Fact]
    public void GetValueFromJsonPath_ValidChildPath_ReturnsValue()
    {
        string json = "{ \"keys\": { \"apiKey\": \"YOUR_KEY_HERE\" } }";
        string path = "keys.apiKey";

        string result = json.GetValueFromJsonPath(path);

        Assert.Equal("YOUR_KEY_HERE", result);
    }

    [Fact]
    public void GetValueFromJsonPath_InvalidPath_ThrowsException()
    {
        string json = "{\"name\":\"John\", \"age\":30}";
        string path = "invalid";

        Assert.Throws<InvalidOperationException>(() => json.GetValueFromJsonPath(path));
    }

    [Fact]
    public void AsSerializedJson_ValidObject_ReturnsJson()
    {
        var obj = new { name = "John", age = 30 };

        string result = obj.AsSerializedJson();

        Assert.Equal("{\"name\":\"John\",\"age\":30}", result);
    }

    [Fact]
    public void AsDeserializedJson_ValidJson_ReturnsObject()
    {
        string json = "{\"name\":\"John\", \"age\":30}";

        var person = json.AsDeserializedJson<Person>();

        Assert.Equal("John", person.Name);
        Assert.Equal(30, person.Age);
    }

    [Fact]
    public void PrettyPrintJson_ValidJson_ReturnsIndentedJson()
    {
        string json = "{\"name\":\"John\", \"age\":30}";

        string result = json.PrettyPrintJson();

        string expected = $"{{{Environment.NewLine}  \"name\": \"John\",{Environment.NewLine}  \"age\": 30{Environment.NewLine}}}";

        Assert.Equal(expected, result);
    }

    [Fact]
    public void PrettyPrintJson_ValidObject_ReturnsIndentedJson()
    {
        var obj = new { name = "John", age = 30 };

        string result = obj.PrettyPrintJson();

        string expected = $"{{{Environment.NewLine}  \"name\": \"John\",{Environment.NewLine}  \"age\": 30{Environment.NewLine}}}";

        Assert.Equal(expected, result);
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}