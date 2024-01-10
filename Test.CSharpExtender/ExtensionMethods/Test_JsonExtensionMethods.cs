using CSharpExtender.ExtensionMethods;
using System.Text.Json;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_JsonExtensionMethods
{
    private Person _personObject = new() { Name = "John", Age = 30 };
    private string _personJsonString = "{\"Name\":\"John\",\"Age\":30}";

    [Fact]
    public void GetValueFromJsonPath_ValidPath_ReturnsValue()
    {
        string path = "Name";

        string result = _personJsonString.GetValueFromJsonPath(path);

        Assert.Equal("John", result);
    }

    [Fact]
    public void GetValueFromComplexJsonPath_ValidPath_ReturnsValue()
    {
        var complexPerson = new ComplexPerson
        {
            Name = new Name { First = "John", Last = "Smith" },
            Age = 30
        };

        Assert.Equal("30",
            complexPerson.AsSerializedJson().GetValueFromJsonPath("Age"));
        Assert.Equal("John", 
            complexPerson.AsSerializedJson().GetValueFromJsonPath("Name.First"));
        Assert.Equal("John Smith", 
            complexPerson.AsSerializedJson().GetValueFromJsonPath("Name.Full"));
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
        string path = "invalid";

        Assert.Throws<InvalidOperationException>(() => 
            _personJsonString.GetValueFromJsonPath(path));
    }

    [Fact]
    public void AsSerializedJson_ValidObject_ReturnsJson()
    {
        string result = _personObject.AsSerializedJson();

        Assert.Equal(_personJsonString, result);
    }

    [Fact]
    public void AsDeserializedJson_ValidJson_ReturnsObject()
    {
        var person = _personJsonString.AsDeserializedJson<Person>();

        Assert.Equal("John", person.Name);
        Assert.Equal(30, person.Age);
    }

    [Fact]
    public void PrettyPrintJson_ValidJson_ReturnsIndentedJson()
    {
        string result = _personJsonString.PrettyPrintJson();

        string expected = 
            $"{{{Environment.NewLine}  \"Name\": \"John\",{Environment.NewLine}  \"Age\": 30{Environment.NewLine}}}";

        Assert.Equal(expected, result);
    }

    [Fact]
    public void PrettyPrintJson_ValidObject_ReturnsIndentedJson()
    {
        string result = _personObject.PrettyPrintJson();

        string expected = $"{{{Environment.NewLine}  \"Name\": \"John\",{Environment.NewLine}  \"Age\": 30{Environment.NewLine}}}";

        Assert.Equal(expected, result);
    }

    [Fact]
    public void PrettyPrintJsonWithOptions_ValidObject_ReturnsIndentedJson()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string result = _personObject.PrettyPrintJson(options);

        string expected = $"{{{Environment.NewLine}  \"name\": \"John\",{Environment.NewLine}  \"age\": 30{Environment.NewLine}}}";

        Assert.Equal(expected, result);
    }

    #region Classes used for testing

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class ComplexPerson
    {
        public Name Name { get; set; } = new();
        public int Age { get; set; }
    }

    public class Name
    { 
        public string First { get; set; } = string.Empty;
        public string Last { get; set; } = string.Empty;
        public string Full => $"{First} {Last}";
    }

    #endregion
}