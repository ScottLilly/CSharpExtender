using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_NumericExtensionMethods
{
    [Fact]
    public void Test_IsEven()
    {
        Assert.True(2.IsEven());
        Assert.False(3.IsEven());
    }

    [Fact]
    public void Test_IsOdd()
    {
        Assert.False(2.IsOdd());
        Assert.True(3.IsOdd());
    }
}