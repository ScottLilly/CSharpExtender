using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

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

    // Worked example: 2, 4, 4, 4, 5, 5, 7, 9 has a mean of 5, a population
    // standard deviation of 2, and a sample standard deviation of sqrt(32/7)
    private static readonly double[] _values = [2, 4, 4, 4, 5, 5, 7, 9];

    [Fact]
    public void PopulationStandardDeviation_ReturnsExpectedValue()
    {
        Assert.Equal(2.0, _values.PopulationStandardDeviation(), 10);
    }

    [Fact]
    public void StandardDeviation_ReturnsSampleValue()
    {
        Assert.Equal(Math.Sqrt(32.0 / 7.0), _values.StandardDeviation(), 10);
    }

    [Fact]
    public void StandardDeviation_IntegerValues_MatchTheDoubleResult()
    {
        int[] integers = [2, 4, 4, 4, 5, 5, 7, 9];

        Assert.Equal(_values.StandardDeviation(), integers.StandardDeviation(), 10);
        Assert.Equal(2.0, integers.PopulationStandardDeviation(), 10);
    }

    [Fact]
    public void StandardDeviation_DecimalValues_MatchTheDoubleResult()
    {
        decimal[] decimals = [2, 4, 4, 4, 5, 5, 7, 9];

        Assert.Equal(_values.StandardDeviation(), decimals.StandardDeviation(), 10);
        Assert.Equal(2.0, decimals.PopulationStandardDeviation(), 10);
    }

    [Fact]
    public void StandardDeviation_IdenticalValues_ReturnsZero()
    {
        double[] identical = [3, 3, 3, 3];

        Assert.Equal(0.0, identical.StandardDeviation(), 10);
        Assert.Equal(0.0, identical.PopulationStandardDeviation(), 10);
    }

    [Fact]
    public void StandardDeviation_FewerThanTwoValues_Throws()
    {
        double[] single = [5];

        Assert.Throws<ArgumentException>(() => single.StandardDeviation());
        Assert.Throws<ArgumentException>(() => Array.Empty<double>().StandardDeviation());
    }

    [Fact]
    public void PopulationStandardDeviation_SingleValue_ReturnsZero()
    {
        double[] single = [5];

        Assert.Equal(0.0, single.PopulationStandardDeviation(), 10);
    }

    [Fact]
    public void PopulationStandardDeviation_NoValues_Throws()
    {
        Assert.Throws<ArgumentException>(() => Array.Empty<double>().PopulationStandardDeviation());
    }

    [Fact]
    public void StandardDeviation_NullCollection_Throws()
    {
        double[] nullValues = null!;

        Assert.Throws<ArgumentNullException>(() => nullValues.StandardDeviation());
        Assert.Throws<ArgumentNullException>(() => nullValues.PopulationStandardDeviation());
    }

    [Fact]
    public void StandardDeviation_LazySource_IsEnumeratedOnce()
    {
        int enumerationCount = 0;

        IEnumerable<double> Counted()
        {
            enumerationCount++;

            foreach (var value in _values)
            {
                yield return value;
            }
        }

        Counted().StandardDeviation();

        Assert.Equal(1, enumerationCount);
    }
}