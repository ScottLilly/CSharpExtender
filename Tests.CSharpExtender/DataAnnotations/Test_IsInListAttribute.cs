using System.ComponentModel.DataAnnotations;
using CSharpExtender.DataAnnotations;

namespace Tests.CSharpExtender.DataAnnotations;

public class Test_IsInListAttribute
{
    private static ValidationContext GetValidationContext(object instance) => new(instance);

    private class ColorModel
    {
        [IsInList("Red", "Green", "Blue")]
        public string Color { get; set; } = string.Empty;
    }

    private class CaseInsensitiveColorModel
    {
        [IsInList("Red", "Green", "Blue", IgnoreCase = true)]
        public string Color { get; set; } = string.Empty;
    }

    private class RatingModel
    {
        [IsInList(1, 2, 3)]
        public long Rating { get; set; }
    }

    private class EmptyListModel
    {
        [IsInList]
        public string Anything { get; set; } = string.Empty;
    }

    [Fact]
    public void IsInListAttribute_ValueInList_ReturnsSuccess()
    {
        var model = new ColorModel { Color = "Green" };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void IsInListAttribute_ValueNotInList_ReturnsError()
    {
        var model = new ColorModel { Color = "Purple" };

        var results = Validate(model);

        Assert.Single(results);
        Assert.Contains("Must be one of: Red, Green, Blue.", results[0].ErrorMessage);
    }

    [Fact]
    public void IsInListAttribute_NullValue_ReturnsSuccess()
    {
        var attribute = new IsInListAttribute("Red", "Green");
        var model = new { Color = null as string };

        var result = attribute.GetValidationResult(model.Color, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsInListAttribute_WrongCase_ReturnsErrorByDefault()
    {
        var model = new ColorModel { Color = "green" };

        var results = Validate(model);

        Assert.Single(results);
    }

    [Fact]
    public void IsInListAttribute_WrongCase_ReturnsSuccessWhenIgnoringCase()
    {
        var model = new CaseInsensitiveColorModel { Color = "green" };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void IsInListAttribute_IntegerLiteralsAgainstALongProperty_ReturnsSuccess()
    {
        // The attribute's values are ints, the property is a long, so a plain
        // Equals would never match
        var model = new RatingModel { Rating = 2 };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void IsInListAttribute_NumericValueNotInList_ReturnsError()
    {
        var model = new RatingModel { Rating = 9 };

        var results = Validate(model);

        Assert.Single(results);
    }

    [Fact]
    public void IsInListAttribute_NoAllowedValues_ReturnsError()
    {
        var model = new EmptyListModel { Anything = "something" };

        var results = Validate(model);

        Assert.Single(results);
        Assert.Contains("at least one allowed value", results[0].ErrorMessage);
    }

    [Fact]
    public void IsInListAttribute_CustomErrorMessage_IsUsed()
    {
        var attribute = new IsInListAttribute("Red") { ErrorMessage = "Pick red." };
        var model = new { Color = "Blue" };

        var result = attribute.GetValidationResult(model.Color, GetValidationContext(model));

        Assert.Equal("Pick red.", result!.ErrorMessage);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, GetValidationContext(model), results, true);

        return results;
    }
}
