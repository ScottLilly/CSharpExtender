using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_NumericExtensionMethods
{
    [Theory]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(-2, true)]
    [InlineData(0, true)]
    public void IsEven_ShouldReturnExpectedResult(int value, bool expectedResult)
    {
        Assert.Equal(expectedResult, value.IsEven());
    }

    [Theory]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(-3, true)]
    [InlineData(0, false)]
    public void IsOdd_ShouldReturnExpectedResult(int value, bool expectedResult)
    {
        Assert.Equal(expectedResult, value.IsOdd());
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(-2, false)]
    [InlineData(0, false)]
    public void IsPositive_ShouldReturnExpectedResult(int value, bool expectedResult)
    {
        Assert.Equal(expectedResult, value.IsPositive());
    }

    [Theory]
    [InlineData(2, false)]
    [InlineData(-2, true)]
    [InlineData(0, false)]
    public void IsNegative_ShouldReturnExpectedResult(int value, bool expectedResult)
    {
        Assert.Equal(expectedResult, value.IsNegative());
    }

    [Theory]
    [InlineData(4, 2, true)]
    [InlineData(5, 2, false)]
    [InlineData(-4, 2, true)]
    [InlineData(0, 2, true)]
    public void IsEvenlyDivisibleBy_ShouldReturnExpectedResult(int value, int divisor, bool expectedResult)
    {
        Assert.Equal(expectedResult, value.IsEvenlyDivisibleBy(divisor));
    }
}