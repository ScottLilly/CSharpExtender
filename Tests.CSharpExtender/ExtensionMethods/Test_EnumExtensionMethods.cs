using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

public enum TestEnum
{
    [Description("Test Description")]
    TestValue,
    [Description("Test Description 2")]
    TestValue2,
    TestValue3
}

public enum DisplayEnum
{
    [Display(Name = "First Display Name")]
    FirstValue,
    [Display(Name = "Second Display Name")]
    SecondValue,
    NoAttribute,

    // Carries a DisplayAttribute that sets everything but Name, so GetName returns null
    [Display(Description = "Has a description but no name")]
    NoNameOnAttribute,

    // Both attributes on one member, to prove the two caches are read independently
    [Display(Name = "Display Wins Here")]
    [Description("Description Wins Here")]
    BothAttributes
}

public enum SecondDisplayEnum
{
    [Display(Name = "Other Enum First Value")]
    FirstValue
}

[Flags]
public enum DisplayFlagsEnum
{
    None = 0,
    [Display(Name = "Read access")]
    Read = 1,
    [Display(Name = "Write access")]
    Write = 2
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
    public void GetEnumDisplayName_ReturnsCorrectDisplayName()
    {
        Assert.Equal("First Display Name", DisplayEnum.FirstValue.GetEnumDisplayName());
        Assert.Equal("Second Display Name", DisplayEnum.SecondValue.GetEnumDisplayName());

        // Test caching
        Assert.Equal("Other Enum First Value", SecondDisplayEnum.FirstValue.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_ReturnsEnumNameIfNoDisplayAttribute()
    {
        Assert.Equal("NoAttribute", DisplayEnum.NoAttribute.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_ReturnsEnumNameIfAttributeSetsNoName()
    {
        // A DisplayAttribute that sets Description but not Name falls back the same
        // way a missing attribute does, rather than returning null
        Assert.Equal("NoNameOnAttribute", DisplayEnum.NoNameOnAttribute.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_ReturnsValueForUndefinedEnumValue()
    {
        var undefined = (DisplayEnum)999;

        Assert.Equal("999", undefined.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_ReturnsCombinedNamesForCombinedFlags()
    {
        var combined = DisplayFlagsEnum.Read | DisplayFlagsEnum.Write;

        Assert.Equal("Read, Write", combined.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_ReturnsDisplayNameForSingleFlag()
    {
        Assert.Equal("Read access", DisplayFlagsEnum.Read.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_TwoEnumsSharingAnUnderlyingValue_DoNotShareACacheEntry()
    {
        // The cache is held per closed enum type rather than keyed on a boxed
        // Enum, so two types whose members are both 0 must not collide
        Assert.Equal(0, (int)DisplayEnum.FirstValue);
        Assert.Equal(0, (int)SecondDisplayEnum.FirstValue);

        Assert.Equal("First Display Name", DisplayEnum.FirstValue.GetEnumDisplayName());
        Assert.Equal("Other Enum First Value", SecondDisplayEnum.FirstValue.GetEnumDisplayName());
        Assert.Equal("First Display Name", DisplayEnum.FirstValue.GetEnumDisplayName());
    }

    [Fact]
    public void GetEnumDisplayName_RepeatedCalls_ReturnTheSameAnswer()
    {
        // Second and later calls come from the cache rather than reflection
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal("Second Display Name", DisplayEnum.SecondValue.GetEnumDisplayName());
            Assert.Equal("NoAttribute", DisplayEnum.NoAttribute.GetEnumDisplayName());
            Assert.Equal("999", ((DisplayEnum)999).GetEnumDisplayName());
        }
    }

    [Fact]
    public void GetEnumDisplayName_DoesNotReadTheDescriptionAttribute()
    {
        // The two caches are separate, so a member carrying both attributes gets
        // the right one from each method
        Assert.Equal("Display Wins Here", DisplayEnum.BothAttributes.GetEnumDisplayName());
        Assert.Equal("Description Wins Here", DisplayEnum.BothAttributes.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDescription_DoesNotReadTheDisplayAttribute()
    {
        // A member with only a DisplayAttribute has no description, so it falls
        // back to its name rather than borrowing the display name
        Assert.Equal("FirstValue", DisplayEnum.FirstValue.GetEnumDescription());
    }

    [Fact]
    public void GetEnumDisplayNames_ReturnsAllDisplayNames()
    {
        var displayNames = EnumExtensionMethods.GetEnumDisplayNames<DisplayEnum>();

        Assert.Contains("First Display Name", displayNames);
        Assert.Contains("Second Display Name", displayNames);
        Assert.Contains("NoAttribute", displayNames);
        Assert.Contains("NoNameOnAttribute", displayNames);
        Assert.Contains("Display Wins Here", displayNames);
    }

    [Fact]
    public void GetEnumDisplayNames_ReturnsOneEntryPerMember()
    {
        var displayNames = EnumExtensionMethods.GetEnumDisplayNames<DisplayEnum>().ToList();

        Assert.Equal(EnumExtensionMethods.GetEnumValues<DisplayEnum>().Count(),
            displayNames.Count);
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