using System.Globalization;
using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

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
        var expected = "2022-01-15T13:45:30.0000000";
        Assert.Equal(expected, date.ToIso8601String());
    }

    [Fact]
    public void ToIso8601String_UtcKind_EndsWithZ()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30, DateTimeKind.Utc);

        Assert.Equal("2022-01-15T13:45:30.0000000Z", date.ToIso8601String());
    }

    [Fact]
    public void ToIso8601String_LocalKind_CarriesOffsetRatherThanZ()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30, DateTimeKind.Local);
        var expectedOffset = TimeZoneInfo.Local.GetUtcOffset(date)
            .ToString(@"hh\:mm");

        var result = date.ToIso8601String();

        Assert.DoesNotContain("Z", result);
        Assert.EndsWith(expectedOffset, result);
    }

    [Fact]
    public void ToIso8601String_UnspecifiedKind_CarriesNoOffsetOrZ()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30, DateTimeKind.Unspecified);

        var result = date.ToIso8601String();

        Assert.DoesNotContain("Z", result);
        Assert.DoesNotContain("+", result);
        Assert.Equal("2022-01-15T13:45:30.0000000", result);
    }

    [Fact]
    public void ToIso8601String_RoundTripsThroughParse()
    {
        var date = new DateTime(2022, 1, 15, 13, 45, 30, DateTimeKind.Utc);

        var parsed = DateTime.Parse(date.ToIso8601String(),
            CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        Assert.Equal(date, parsed);
        Assert.Equal(DateTimeKind.Utc, parsed.Kind);
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