using CSharpExtender.ExtensionMethods;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_DisplayFormatExtensionMethods
{
    private class Order
    {
        [DisplayFormat(DataFormatString = "yyyy-MM-dd HH:mm")]
        public DateTime CreatedDate { get; set; } = new(2026, 1, 2, 3, 4, 5);

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; } = 1234.5m;

        [DisplayFormat(DataFormatString = "yyyy-MM-dd", NullDisplayText = "Not shipped")]
        public DateTime? ShippedDate { get; set; }

        [DisplayFormat]
        public string Reference { get; set; } = "ABC-123";

        public string CustomerName { get; set; } = "Scott Lilly";
    }

    [Fact]
    public void ToDisplayString_BareFormatSpecifier_IsApplied()
    {
        var order = new Order();

        Assert.Equal("2026-01-02 03:04",
            order.ToDisplayString(nameof(Order.CreatedDate), CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToDisplayString_WrappedFormatString_IsApplied()
    {
        var order = new Order();

        var result = order.ToDisplayString(nameof(Order.Total),
            CultureInfo.GetCultureInfo("en-US"));

        Assert.Equal("$1,234.50", result);
    }

    [Fact]
    public void ToDisplayString_UsesTheSuppliedCulture()
    {
        var order = new Order();

        var result = order.ToDisplayString(nameof(Order.Total),
            CultureInfo.GetCultureInfo("de-DE"));

        Assert.Contains("1.234,50", result);
    }

    [Fact]
    public void ToDisplayString_NullValueWithNullDisplayText_UsesThatText()
    {
        var order = new Order { ShippedDate = null };

        Assert.Equal("Not shipped", order.ToDisplayString(nameof(Order.ShippedDate)));
    }

    [Fact]
    public void ToDisplayString_AttributeWithNoFormatString_FallsBackToToString()
    {
        var order = new Order();

        Assert.Equal("ABC-123", order.ToDisplayString(nameof(Order.Reference)));
    }

    [Fact]
    public void ToDisplayString_PropertyWithoutTheAttribute_FallsBackToToString()
    {
        var order = new Order();

        Assert.Equal("Scott Lilly", order.ToDisplayString(nameof(Order.CustomerName)));
    }

    [Fact]
    public void ToDisplayString_UnknownProperty_Throws()
    {
        var order = new Order();

        Assert.Throws<ArgumentException>(() => order.ToDisplayString("NoSuchProperty"));
    }

    [Fact]
    public void ToDisplayString_NullObject_Throws()
    {
        object nullObject = null;

        Assert.Throws<ArgumentNullException>(() => nullObject.ToDisplayString("Anything"));
    }

    [Fact]
    public void ToDisplayStrings_ReturnsOnlyFormattedProperties()
    {
        var order = new Order();

        var formatted = order.ToDisplayStrings(CultureInfo.InvariantCulture);

        Assert.Equal(4, formatted.Count);
        Assert.Equal("2026-01-02 03:04", formatted[nameof(Order.CreatedDate)]);
        Assert.Equal("Not shipped", formatted[nameof(Order.ShippedDate)]);
        Assert.False(formatted.ContainsKey(nameof(Order.CustomerName)));
    }
}
