using CSharpExtender.Services;
using System.ComponentModel;
using System.Reflection;

namespace Test.CSharpExtender.Services;

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

    public static string StaticMethod() => "Static";
}

public class OverloadedMethodClass
{
    public string Go(string a) => $"string:{a}";

    public string Go(int a) => $"int:{a}";

    public string Go(string a, string b) => $"two:{a}{b}";

    // Neither overload is an exact match for an int argument, so the call cannot
    // be resolved by type or by argument count
    public string Widening(long a) => $"long:{a}";

    public string Widening(double a) => $"double:{a}";
}

[Description("Class description")]
public class TestClassWithAttribute
{
    public string Name { get; set; } = "TestName";
}

#endregion

public class Test_SmartReflection
{
    private readonly TestClass _testObject = new();
    private readonly Type _testType = typeof(TestClass);

    [Fact]
    public void HasAttribute_ReturnsTrue_WhenAttributeExists()
    {
        bool result = SmartReflection.HasAttribute<DescriptionAttribute>(typeof(TestClassWithAttribute));
        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_ReturnsFalse_WhenAttributeDoesNotExist()
    {
        bool result = SmartReflection.HasAttribute<ObsoleteAttribute>(typeof(TestClassWithAttribute));
        Assert.False(result);
    }

    [Fact]
    public void HasPropertyAttribute_ReturnsTrue_WhenPropertyAttributeExists()
    {
        bool result = SmartReflection.HasPropertyAttribute<DescriptionAttribute>(_testType, nameof(TestClass.Name));
        Assert.True(result);
    }

    [Fact]
    public void HasPropertyAttribute_ReturnsFalse_WhenPropertyAttributeDoesNotExist()
    {
        bool result = SmartReflection.HasPropertyAttribute<ObsoleteAttribute>(_testType, nameof(TestClass.Name));
        Assert.False(result);
    }

    [Fact]
    public void HasPropertyAttribute_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            SmartReflection.HasPropertyAttribute<DescriptionAttribute>(_testType, "NonExistent"));
    }

    [Fact]
    public void GetPropertiesWithAttribute_ReturnsProperties_WhenAttributeExists()
    {
        PropertyInfo[] properties = SmartReflection.GetPropertiesWithAttribute<DescriptionAttribute>(_testType);
        Assert.Single(properties);
        Assert.Equal("Name", properties[0].Name);
    }

    [Fact]
    public void GetPropertiesWithAttribute_ReturnsEmpty_WhenNoPropertiesHaveAttribute()
    {
        PropertyInfo[] properties = SmartReflection.GetPropertiesWithAttribute<SerializableAttribute>(_testType);
        Assert.Empty(properties);
    }

    [Fact]
    public void GetPropertyValue_ReturnsValue_WhenPropertyExists()
    {
        string value = SmartReflection.GetPropertyValue<string>(_testObject, nameof(TestClass.Name));
        Assert.Equal("TestName", value);
    }

    [Fact]
    public void GetPropertyValue_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            SmartReflection.GetPropertyValue<string>(_testObject, "NonExistent"));
    }

    [Fact]
    public void GetPropertyValue_Throws_WhenInvalidType()
    {
        Assert.Throws<InvalidCastException>(() =>
            SmartReflection.GetPropertyValue<int>(_testObject, nameof(TestClass.Name)));
    }

    [Fact]
    public void GetPropertyValue_Throws_WhenObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SmartReflection.GetPropertyValue<string>(null, nameof(TestClass.Name)));
    }

    [Fact]
    public void SetPropertyValue_SetsValue_WhenPropertyExists()
    {
        SmartReflection.SetPropertyValue(_testObject, nameof(TestClass.Name), "NewName");
        Assert.Equal("NewName", _testObject.Name);
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            SmartReflection.SetPropertyValue(_testObject, "NonExistent", "Value"));
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenPropertyIsReadOnly()
    {
        Assert.Throws<InvalidOperationException>(() =>
            SmartReflection.SetPropertyValue(_testObject, "ReadOnlyProperty", "Value"));
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenInvalidType()
    {
        Assert.Throws<InvalidCastException>(() =>
            SmartReflection.SetPropertyValue(_testObject, nameof(TestClass.Name), 42));
    }

    [Fact]
    public void SetPropertyValue_Throws_WhenObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SmartReflection.SetPropertyValue<string>(null, nameof(TestClass.Name), "Value"));
    }

    [Fact]
    public void GetPropertyNames_ReturnsAllPublicProperties()
    {
        string[] names = SmartReflection.GetPropertyNames(_testType);
        Assert.Equal(new[]
        {
            nameof(TestClass.Name),
            nameof(TestClass.Number),
            nameof(TestClass.ObsoleteProperty),
            nameof(TestClass.ReadOnlyProperty)
        }, names);
    }

    [Fact]
    public void HasProperty_ReturnsTrue_WhenPropertyExists()
    {
        bool result = SmartReflection.HasProperty(_testType, nameof(TestClass.Name));
        Assert.True(result);
    }

    [Fact]
    public void HasProperty_ReturnsFalse_WhenPropertyDoesNotExist()
    {
        bool result = SmartReflection.HasProperty(_testType, "NonExistent");
        Assert.False(result);
    }

    [Fact]
    public void GetPropertyType_ReturnsType_WhenPropertyExists()
    {
        Type type = SmartReflection.GetPropertyType(_testType, nameof(TestClass.Name));
        Assert.Equal(typeof(string), type);
    }

    [Fact]
    public void GetPropertyType_Throws_WhenPropertyDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            SmartReflection.GetPropertyType(_testType, "NonExistent"));
    }

    [Fact]
    public void InvokeMethod_ReturnsResult_WhenMethodExists()
    {
        string result = SmartReflection.InvokeMethod<string>(_testObject, "GetGreeting");
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void InvokeMethod_InvokesPrivateMethod_WhenMethodExists()
    {
        int result = SmartReflection.InvokeMethod<int>(_testObject, "PrivateMethod", 5);
        Assert.Equal(10, result);
    }

    [Fact]
    public void InvokeMethod_Throws_WhenMethodDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() =>
            SmartReflection.InvokeMethod<string>(_testObject, "NonExistent"));
    }

    [Fact]
    public void InvokeMethod_Throws_WhenInvalidReturnType()
    {
        Assert.Throws<InvalidCastException>(() =>
            SmartReflection.InvokeMethod<int>(_testObject, "GetGreeting"));
    }

    [Fact]
    public void InvokeMethod_Throws_WhenObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SmartReflection.InvokeMethod<string>(null, "GetGreeting"));
    }

    [Fact]
    public void InvokeMethod_SelectsOverload_MatchingArgumentTypes()
    {
        var obj = new OverloadedMethodClass();

        Assert.Equal("string:x", SmartReflection.InvokeMethod<string>(obj, "Go", "x"));
        Assert.Equal("int:7", SmartReflection.InvokeMethod<string>(obj, "Go", 7));
    }

    [Fact]
    public void InvokeMethod_SelectsOverload_MatchingArgumentCount()
    {
        var obj = new OverloadedMethodClass();

        Assert.Equal("two:xy", SmartReflection.InvokeMethod<string>(obj, "Go", "x", "y"));
    }

    [Fact]
    public void InvokeMethod_SelectsOverload_ByWideningConversion()
    {
        var obj = new OverloadedMethodClass();

        Assert.Equal("long:7", SmartReflection.InvokeMethod<string>(obj, "Widening", 7));
    }

    [Fact]
    public void InvokeMethod_Throws_WhenNoOverloadTakesThatManyArguments()
    {
        var obj = new OverloadedMethodClass();

        Assert.Throws<ArgumentException>(() =>
            SmartReflection.InvokeMethod<string>(obj, "Go", "x", "y", "z"));
    }

    [Fact]
    public void InvokeMethod_Throws_WhenANullArgumentLeavesOverloadsAmbiguous()
    {
        // A null argument has no runtime type, so Go(string) and Go(int) cannot
        // be told apart
        var obj = new OverloadedMethodClass();

        Assert.Throws<ArgumentException>(() =>
            SmartReflection.InvokeMethod<string>(obj, "Go", [null]));
    }

    [Fact]
    public void InvokeMethod_InvokesStaticMethod()
    {
        Assert.Equal("Static", SmartReflection.InvokeMethod<string>(_testObject, "StaticMethod"));
    }
}