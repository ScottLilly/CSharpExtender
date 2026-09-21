using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CSharpExtender.DataAnnotations;

namespace Tests.CSharpExtender.DataAnnotations;

public class Test_NumericOnlyAttribute
{
    // Helper method to validate a value with the attribute
    private static ValidationResult? Validate(NumericOnlyAttribute attribute, object? value)
    {
        var context = new ValidationContext(new object());

        return attribute.GetValidationResult(value, context);
    }

    // Helper method to validate under a named culture, so a test that turns on a
    // separator is not answering a question about the machine it runs on
    private static ValidationResult? ValidateUnderCulture(
        NumericOnlyAttribute attribute, object? value, string cultureName)
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);

            return Validate(attribute, value);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void NullInput_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Validate(new NumericOnlyAttribute(), null);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void EmptyString_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Validate(new NumericOnlyAttribute(), "");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("007")]
    [InlineData("9999999999999")]
    public void DigitsOnly_ReturnsSuccess(string input)
    {
        // Arrange & Act
        var result = ValidateUnderCulture(new NumericOnlyAttribute(), input, "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12a")]
    [InlineData("1 2")]
    [InlineData(" 42")]
    [InlineData("42 ")]
    [InlineData(" ")]
    [InlineData("-")]
    [InlineData(".")]
    [InlineData("1e5")]
    [InlineData("(42)")]
    public void NonNumericStrings_ReturnsDefaultErrorMessage(string input)
    {
        // Arrange & Act
        var result = ValidateUnderCulture(new NumericOnlyAttribute(), input, "en-US");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Must contain only numbers.", result.ErrorMessage);
    }

    [Theory]
    [InlineData("9.99")]
    [InlineData("1,234")]
    [InlineData("$9.99")]
    [InlineData("-42")]
    public void OptionsOff_RejectsAnythingButDigits(string input)
    {
        // Arrange & Act
        var result = ValidateUnderCulture(new NumericOnlyAttribute(), input, "en-US");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AllowDecimal_AcceptsTheCulturesDecimalSeparator()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { AllowDecimal = true };

        // Act
        var result = ValidateUnderCulture(attribute, "9.99", "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void AllowDecimal_RejectsAnotherCulturesDecimalSeparator()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { AllowDecimal = true };

        // Act
        var result = ValidateUnderCulture(attribute, "9,99", "en-US");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AllowThousandsSeparator_AcceptsGroupedDigits()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { AllowThousandsSeparator = true };

        // Act
        var result = ValidateUnderCulture(attribute, "1,234,567", "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void FrenchCulture_AcceptsItsOwnSeparators()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute
        {
            AllowDecimal = true,
            AllowThousandsSeparator = true
        };

        // fr-FR groups with a no-break space rather than a comma, so the separators are
        // read from the culture rather than written here, which is the whole point
        NumberFormatInfo french = CultureInfo.GetCultureInfo("fr-FR").NumberFormat;
        string input =
            $"1{french.NumberGroupSeparator}234{french.NumberDecimalSeparator}56";

        // Act
        var result = ValidateUnderCulture(attribute, input, "fr-FR");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void AllowCurrencySymbol_AcceptsTheCulturesSymbol()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute
        {
            AllowCurrencySymbol = true,
            AllowDecimal = true
        };

        // Act
        var result = ValidateUnderCulture(attribute, "$9.99", "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("-42")]
    [InlineData("+42")]
    public void AllowNegative_AcceptsALeadingSign(string input)
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { AllowNegative = true };

        // Act
        var result = ValidateUnderCulture(attribute, input, "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void AllOptionsOn_AcceptsAFormattedCurrencyValue()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute
        {
            AllowDecimal = true,
            AllowThousandsSeparator = true,
            AllowCurrencySymbol = true,
            AllowNegative = true
        };

        // Act
        var result = ValidateUnderCulture(attribute, "-$1,234.56", "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GermanCulture_AcceptsACommaAsTheDecimalSeparator()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { AllowDecimal = true };

        // Act
        var result = ValidateUnderCulture(attribute, "9,99", "de-DE");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UseInvariantCulture_ReadsTheValueTheSameWayUnderAnyCulture()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute
        {
            AllowDecimal = true,
            UseInvariantCulture = true
        };

        // Act
        var period = ValidateUnderCulture(attribute, "9.99", "de-DE");
        var comma = ValidateUnderCulture(attribute, "9,99", "de-DE");

        // Assert
        Assert.Equal(ValidationResult.Success, period);
        Assert.NotNull(comma);
    }

    [Theory]
    [InlineData("٤٢")] // Arabic-Indic digits, which char.IsDigit accepts
    [InlineData("１２３")] // Fullwidth digits
    public void NonAsciiDigits_ReturnsFailure(string input)
    {
        // Arrange & Act
        var result = ValidateUnderCulture(new NumericOnlyAttribute(), input, "en-US");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void MoreDigitsThanADecimalHolds_NoOptions_ReturnsSuccess()
    {
        // Arrange
        string input = new string('9', 30);

        // Act
        var result = ValidateUnderCulture(new NumericOnlyAttribute(), input, "en-US");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void MoreDigitsThanADecimalHolds_WithAnOptionOn_ReturnsFailure()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { AllowDecimal = true };
        string input = new string('9', 30);

        // Act
        var result = ValidateUnderCulture(attribute, input, "en-US");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void InvalidString_ReturnsCustomErrorMessage()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute { ErrorMessage = "Digits only, please." };

        // Act
        var result = ValidateUnderCulture(attribute, "12a", "en-US");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Digits only, please.", result.ErrorMessage);
    }

    [Fact]
    public void NonStringValue_ThrowsInvalidCastException()
    {
        // Arrange
        var attribute = new NumericOnlyAttribute();

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => Validate(attribute, 42));
    }
}
