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

    [Fact]
    public void ApproximatelyEquals_ReturnsTrue_ForIdenticalValues()
    {
        float a = 1.0f;
        float b = 1.0f;

        bool result = a.ApproximatelyEquals(b);

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsTrue_ForValuesWithinDefaultTolerance()
    {
        float a = 100.0f;
        float b = 100.00005f;

        bool result = a.ApproximatelyEquals(b);

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsFalse_ForValuesOutsideDefaultTolerance()
    {
        float a = 100.0f;
        float b = 100.1f;

        bool result = a.ApproximatelyEquals(b);

        Assert.False(result);
    }

    [Fact]
    public void ApproximatelyEquals_UsesCustomTolerance()
    {
        float a = 100.0f;
        float b = 101.0f;

        bool result = a.ApproximatelyEquals(b, 0.02f); // 2% tolerance

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsFalse_WhenNaNComparedToNumber()
    {
        float a = float.NaN;
        float b = 1.0f;

        bool result = a.ApproximatelyEquals(b);

        Assert.False(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsFalse_WhenBothAreNaN()
    {
        float a = float.NaN;
        float b = float.NaN;

        bool result = a.ApproximatelyEquals(b);

        Assert.False(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsTrue_WhenBothArePositiveInfinity()
    {
        float a = float.PositiveInfinity;
        float b = float.PositiveInfinity;

        bool result = a.ApproximatelyEquals(b);

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsTrue_WhenBothAreNegativeInfinity()
    {
        float a = float.NegativeInfinity;
        float b = float.NegativeInfinity;

        bool result = a.ApproximatelyEquals(b);

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsFalse_WhenOneIsInfinityAndOtherIsNot()
    {
        float a = float.PositiveInfinity;
        float b = 100.0f;

        bool result = a.ApproximatelyEquals(b);

        Assert.False(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsTrue_ForZeroAndNearZeroWithinTolerance()
    {
        float a = 0.0f;
        float b = 0.00000001f;

        bool result = a.ApproximatelyEquals(b, 0.0001f);

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsFalse_ForZeroAndLargerValueOutsideTolerance()
    {
        float a = 0.0f;
        float b = 1.0f;

        bool result = a.ApproximatelyEquals(b);

        Assert.False(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsTrue_ForNegativeValuesWithinTolerance()
    {
        float a = -100.0f;
        float b = -100.00005f;

        bool result = a.ApproximatelyEquals(b);

        Assert.True(result);
    }

    [Fact]
    public void ApproximatelyEquals_ReturnsFalse_ForNegativeAndPositiveValue()
    {
        float a = -100.0f;
        float b = 100.0f;

        bool result = a.ApproximatelyEquals(b);

        Assert.False(result);
    }
}