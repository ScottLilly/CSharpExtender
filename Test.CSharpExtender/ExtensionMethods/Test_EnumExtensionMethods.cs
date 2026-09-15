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

public enum SecondEnum
{
    [Description("Second Enum First Option")]
    FirstOption,
    [Description("Second Enum Second Option")]
    SecondOption
}

[Flags]
public enum FlagsEnum
{
    None = 0,
    [Description("Read access")]
    Read = 1,
    [Description("Write access")]
    Write = 2
}

public class Test_EnumExtensionMethods
{
    [Fact]
    public void GetEnumDescription_ReturnsCorrectDescription()
    {
        Assert.Equal("Test Description", 
            TestEnum.TestValue.GetEnumDescription());
        Assert.Equal("Test Description 2",
            TestEnum.TestValue2.GetEnumDescription());

        // Test caching
        Assert.Equal("Second Enum First Option",
            SecondEnum.FirstOption.GetEnumDescription());
        Assert.Equal("Second Enum Second Option",
            SecondEnum.SecondOption.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_ReturnsEnumNameIfNoDescription()
    {
        Assert.Equal("TestValue3",
            TestEnum.TestValue3.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_ReturnsValueForUndefinedEnumValue()
    {
        var undefined = (TestEnum)999;

        Assert.Equal("999", undefined.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_ReturnsCombinedNamesForCombinedFlags()
    {
        var combined = FlagsEnum.Read | FlagsEnum.Write;

        Assert.Equal("Read, Write", combined.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_ReturnsDescriptionForSingleFlag()
    {
        Assert.Equal("Read access", FlagsEnum.Read.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_TwoEnumsSharingAnUnderlyingValue_DoNotShareACacheEntry()
    {
        // The cache is held per closed enum type rather than keyed on a boxed
        // Enum, so two types whose members are both 0 must not collide
        Assert.Equal(0, (int)TestEnum.TestValue);
        Assert.Equal(0, (int)SecondEnum.FirstOption);

        Assert.Equal("Test Description", TestEnum.TestValue.GetEnumDescription());
        Assert.Equal("Second Enum First Option", SecondEnum.FirstOption.GetEnumDescription());
        Assert.Equal("Test Description", TestEnum.TestValue.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_RepeatedCalls_ReturnTheSameAnswer()
    {
        // Second and later calls come from the cache rather than reflection
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal("Test Description 2", TestEnum.TestValue2.GetEnumDescription());
            Assert.Equal("TestValue3", TestEnum.TestValue3.GetEnumDescription());
            Assert.Equal("999", ((TestEnum)999).GetEnumDescription());
        }
    }

    [Fact]
    public void GetEnumValues_CannotBeWrittenThrough()
    {
        // Every caller gets the same instance, so it has to refuse to be changed
        var values = EnumExtensionMethods.GetEnumValues<TestEnum>();

        Assert.Throws<NotSupportedException>(() => ((IList<TestEnum>)values).Add(default));
        Assert.Throws<NotSupportedException>(() => ((IList<TestEnum>)values)[0] = default);
        Assert.Throws<InvalidCastException>(() => (TestEnum[])values);
    }

    [Fact]
    public void GetEnumValues_ReturnsTheSameInstanceEveryCall()
    {
        Assert.Same(EnumExtensionMethods.GetEnumValues<TestEnum>(),
            EnumExtensionMethods.GetEnumValues<TestEnum>());

        Assert.NotSame(EnumExtensionMethods.GetEnumValues<TestEnum>(),
            EnumExtensionMethods.GetEnumValues<SecondEnum>());
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