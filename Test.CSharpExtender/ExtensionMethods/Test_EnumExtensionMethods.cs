using CSharpExtender.ExtensionMethods;
using System.ComponentModel;

namespace Test.CSharpExtender.ExtensionMethods;

public enum TestEnum
{
    [Description("Test Description")]
    TestValue,
    [Description("Test Description 2")]
    TestValue2,
    TestValue3
}

public class EnumExtensionMethodsTests
{
    [Fact]
    public void GetEnumDescription_ReturnsCorrectDescription()
    {
        Assert.Equal("Test Description", 
            TestEnum.TestValue.GetEnumDescription());
        Assert.Equal("Test Description 2",
            TestEnum.TestValue2.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_ReturnsEnumNameIfNoDescription()
    {
        Assert.Equal("TestValue3",
            TestEnum.TestValue3.GetEnumDescription());
    }

    [Fact]
    public void GetEnumValues_ReturnsAllValues()
    {
        var values = EnumExtensionMethods.GetEnumValues<TestEnum>();

        Assert.Contains(TestEnum.TestValue, values);
        Assert.Contains(TestEnum.TestValue2, values);
        Assert.Contains(TestEnum.TestValue3, values);
    }

    [Fact]
    public void GetEnumDescriptions_ReturnsAllDescriptions()
    {
        var descriptions = EnumExtensionMethods.GetEnumDescriptions<TestEnum>();

        Assert.Contains("Test Description", descriptions);
        Assert.Contains("Test Description 2", descriptions);
        Assert.Contains("TestValue3", descriptions);
    }

    [Fact]
    public void ParseEnum_ReturnsCorrectEnumValue()
    {
        Assert.Equal(TestEnum.TestValue, 
            EnumExtensionMethods.ParseEnum<TestEnum>("TestValue"));
        Assert.Equal(TestEnum.TestValue2,
            EnumExtensionMethods.ParseEnum<TestEnum>("TestValue2"));
        Assert.Equal(TestEnum.TestValue3,
            EnumExtensionMethods.ParseEnum<TestEnum>("TestValue3"));
    }

    [Fact]
    public void ParseEnum_IgnoresCase()
    {
        Assert.Equal(TestEnum.TestValue, 
            EnumExtensionMethods.ParseEnum<TestEnum>("testvalue"));
    }

    [Fact]
    public void ParseEnum_ThrowsExceptionOnInvalidValue()
    {
        Assert.Throws<ArgumentException>(() => 
            EnumExtensionMethods.ParseEnum<TestEnum>("InvalidValue"));
    }
}