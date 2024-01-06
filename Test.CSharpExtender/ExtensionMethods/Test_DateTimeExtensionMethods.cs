using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_DateTimeExtensionMethods
{
    [Fact]
    public void StartOfDay_ShouldReturnStartOfDay()
    {
        var date = new DateTime(2022, 1, 1, 13, 45, 30);
        var expected = new DateTime(2022, 1, 1, 0, 0, 0);
        Assert.Equal(expected, date.StartOfDay());
    }

    [Fact]
    public void EndOfDay_ShouldReturnEndOfDay()
    {
        var date = new DateTime(2022, 1, 1, 13, 45, 30);
        var expected = new DateTime(2022, 1, 1, 23, 59, 59);
        Assert.Equal(expected, date.EndOfDay());
    }

    [Fact]
    public void StartOfMonth_ShouldReturnStartOfMonth()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30);
        var expected = new DateTime(2022, 1, 1);
        Assert.Equal(expected, date.StartOfMonth());
    }

    [Fact]
    public void EndOfMonth_ShouldReturnEndOfMonth()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30);
        var expected = new DateTime(2022, 1, 31, 23, 59, 59);
        Assert.Equal(expected, date.EndOfMonth());
    }

    [Fact]
    public void ToIso8601String_ShouldReturnIso8601String()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30);
        var expected = "2022-01-15T13:45:30.000Z";
        Assert.Equal(expected, date.ToIso8601String());
    }

    [Fact]
    public void IsWeekend_ShouldReturnTrueForWeekend()
    {
        var date = new DateTime(2022, 1, 1); // Saturday
        Assert.True(date.IsWeekend());
    }

    [Fact]
    public void IsWeekend_ShouldReturnFalseForWeekday()
    {
        var date = new DateTime(2022, 1, 3); // Monday
        Assert.False(date.IsWeekend());
    }

    [Fact]
    public void IsWeekday_ShouldReturnTrueForWeekday()
    {
        var date = new DateTime(2022, 1, 3); // Monday
        Assert.True(date.IsWeekday());
    }

    [Fact]
    public void IsWeekday_ShouldReturnFalseForWeekend()
    {
        var date = new DateTime(2022, 1, 1); // Saturday
        Assert.False(date.IsWeekday());
    }

    [Fact]
    public void NextDay_ShouldReturnNextDay()
    {
        var date = new DateTime(2022, 1, 1);
        var expected = new DateTime(2022, 1, 2);
        Assert.Equal(expected, date.NextDay());
    }

    [Fact]
    public void PreviousDay_ShouldReturnPreviousDay()
    {
        var date = new DateTime(2022, 1, 2);
        var expected = new DateTime(2022, 1, 1);
        Assert.Equal(expected, date.PreviousDay());
    }
}