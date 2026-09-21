using System.ComponentModel;
using System.Reflection;
using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

#region Classes for Testing

public class TestClass
{
    [Description("Test description")]
    public string Name { get; set; } = "TestName";

    public int Number { get; set; } = 42;

    [Obsolete]
    public string ObsoleteProperty { get; set; } = "Obsolete";

    public string ReadOnlyProperty { get; } = "ReadOnly";

    public string GetGreeting() => "Hello";

    private int PrivateMethod(int x) => x * 2;
}

[Description("Class description")]
public class TestClassWithAttribute
{
    public string Name { get; set; } = "TestName";
}

#endregion

public class Test_SmartReflectionExtensionMethods
{
    private readonly TestClass _testObj = new();
    private readonly TestClassWithAttribute _testObjWithAttr = new();

    [Fact]
    public void HasAttribute_ReturnsTrue_WhenAttributeExists()
    {
        bool result = _testObjWithAttr.HasAttribute<DescriptionAttribute>();
        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_ReturnsFalse_WhenAttributeDoesNotExist()
    {
        bool result = _testObjWithAttr.HasAttribute<ObsoleteAttribute>();
        Assert.False(result);
    }

    [Fact]
    public void HasAttribute_Throws_WhenObjectIsNull()
    {
        TestClassWithAttribute nullObj = null!;
        Assert.Throws<ArgumentNullException>(() => nullObj.HasAttribute<DescriptionAttribute>());
    }

    [Fact]
    public void HasPropertyAttribute_ReturnsTrue_WhenPropertyAttributeExists()
    {
        bool result = _testObj.HasPropertyAttribute<DescriptionAttribute>("Name");
        Assert.True(result);
    }

    [Fact]
    public void HasPropertyAttribute_ReturnsFalse_WhenPropertyAttributeDoesNotExist()
    {
        bool result = _testObj.HasPropertyAttribute<ObsoleteAttribute>("Name");
        Assert.False(result);
    }

    [Fact]
    public void HasPropertyAttribute_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            _testObj.HasPropertyAttribute<DescriptionAttribute>("NonExistent"));
    }

    [Fact]
    public void HasPropertyAttribute_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.HasPropertyAttribute<DescriptionAttribute>("Name"));
    }

    [Fact]
    public void GetPropertiesWithAttribute_ReturnsProperties_WhenAttributeExists()
    {
        PropertyInfo[] properties = _testObj.GetPropertiesWithAttribute<DescriptionAttribute>();
        Assert.Single(properties);
        Assert.Equal("Name", properties[0].Name);
    }

    [Fact]
    public void GetPropertiesWithAttribute_ReturnsEmpty_WhenNoPropertiesHaveAttribute()
    {
        PropertyInfo[] properties = _testObj.GetPropertiesWithAttribute<SerializableAttribute>();
        Assert.Empty(properties);
    }

    [Fact]
    public void GetPropertiesWithAttribute_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.GetPropertiesWithAttribute<DescriptionAttribute>());
    }

    [Fact]
    public void GetPropertyValue_ReturnsValue_WhenPropertyExists()
    {
        string? value = _testObj.GetPropertyValue<string>("Name");
        Assert.Equal("TestName", value);
    }

    [Fact]
    public void GetPropertyValue_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            _testObj.GetPropertyValue<string>("NonExistent"));
    }

    [Fact]
    public void GetPropertyValue_Throws_WhenInvalidType()
    {
        Assert.Throws<InvalidCastException>(() =>
            _testObj.GetPropertyValue<int>("Name"));
    }

    [Fact]
    public void GetPropertyValue_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.GetPropertyValue<string>("Name"));
    }

    [Fact]
    public void SetPropertyValue_SetsValue_WhenPropertyExists()
    {
        _testObj.SetPropertyValue("Name", "NewName");
        Assert.Equal("NewName", _testObj.Name);
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            _testObj.SetPropertyValue("NonExistent", "Value"));
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenPropertyIsReadOnly()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _testObj.SetPropertyValue("ReadOnlyProperty", "Value"));
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenInvalidType()
    {
        Assert.Throws<InvalidCastException>(() =>
            _testObj.SetPropertyValue("Name", 42));
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.SetPropertyValue("Name", "Value"));
    }

    [Fact]
    public void GetPropertyNames_ReturnsAllPublicProperties()
    {
        string[] names = _testObj.GetPropertyNames();
        Assert.Equal(new[] { "Name", "Number", "ObsoleteProperty", "ReadOnlyProperty" }, names);
    }

    [Fact]
    public void GetPropertyNames_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.GetPropertyNames());
    }

    [Fact]
    public void HasProperty_ReturnsTrue_WhenPropertyExists()
    {
        bool result = _testObj.HasProperty("Name");
        Assert.True(result);
    }

    [Fact]
    public void HasProperty_ReturnsFalse_WhenPropertyDoesNotExist()
    {
        bool result = _testObj.HasProperty("NonExistent");
        Assert.False(result);
    }

    [Fact]
    public void HasProperty_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.HasProperty("Name"));
    }

    [Fact]
    public void GetPropertyType_ReturnsType_WhenPropertyExists()
    {
        Type type = _testObj.GetPropertyType("Name");
        Assert.Equal(typeof(string), type);
    }

    [Fact]
    public void GetPropertyType_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            _testObj.GetPropertyType("NonExistent"));
    }

    [Fact]
    public void GetPropertyType_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.GetPropertyType("Name"));
    }

    [Fact]
    public void InvokeMethod_ReturnsResult_WhenMethodExists()
    {
        string? result = _testObj.InvokeMethod<string>("GetGreeting");
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void InvokeMethod_InvokesPrivateMethod_WhenMethodExists()
    {
        int result = _testObj.InvokeMethod<int>("PrivateMethod", 5);
        Assert.Equal(10, result);
    }

    [Fact]
    public void InvokeMethod_Throws_WhenMethodDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            _testObj.InvokeMethod<string>("NonExistent"));
    }

    [Fact]
    public void InvokeMethod_Throws_WhenInvalidReturnType()
    {
        Assert.Throws<InvalidCastException>(() =>
            _testObj.InvokeMethod<int>("GetGreeting"));
    }

    [Fact]
    public void InvokeMethod_Throws_WhenObjectIsNull()
    {
        TestClass nullObj = null!;
        Assert.Throws<ArgumentNullException>(() =>
            nullObj.InvokeMethod<string>("GetGreeting"));
    }
}