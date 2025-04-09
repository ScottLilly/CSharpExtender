using CSharpExtender.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Test.CSharpExtender.DataAnnotations;

public class Test_AlphaOnlyAttribute
{
    // Helper method to validate an input string with the attribute
    private static ValidationResult Validate(string? input, string? errorMessage = null)
    {
        var attribute = errorMessage == null
            ? new AlphaOnlyAttribute()
            : new AlphaOnlyAttribute { ErrorMessage = errorMessage };

        var context = new ValidationContext(new object());

        return attribute.GetValidationResult(input, context);
    }

    [Fact]
    public void NullInput_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Validate(null);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void EmptyString_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Validate("");

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("ABC")]
    [InlineData("HelloWorld")]
    [InlineData("a")]
    [InlineData("Z")]
    public void ValidAlphaOnlyStrings_ReturnsSuccess(string input)
    {
        // Arrange & Act
        var result = Validate(input);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("123", "Must contain only letters.")]
    [InlineData("abc123", "Must contain only letters.")]
    [InlineData("abc 123", "Must contain only letters.")]
    [InlineData("abc!", "Must contain only letters.")]
    [InlineData(" ", "Must contain only letters.")]
    [InlineData("@#$%", "Must contain only letters.")]
    public void InvalidStrings_ReturnsDefaultErrorMessage(string input, string expectedError)
    {
        // Arrange & Act
        var result = Validate(input);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal(expectedError, result.ErrorMessage);
    }

    [Theory]
    [InlineData("abc123", "Only letters allowed here.")]
    [InlineData("abc 123", "Only letters allowed here.")]
    [InlineData("123", "Only letters allowed here.")]
    public void InvalidStrings_ReturnsCustomErrorMessage(string input, string customError)
    {
        // Arrange & Act
        var result = Validate(input, customError);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal(customError, result.ErrorMessage);
    }

    [Fact]
    public void MixedCaseString_ReturnsSuccess()
    {
        // Arrange
        string input = "HelloWorld123".Substring(0, 10);

        // Act
        var result = Validate(input);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void SingleNonLetterAtStart_ReturnsFailure()
    {
        // Arrange
        string input = "1abc";

        // Act
        var result = Validate(input);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Must contain only letters.", result.ErrorMessage);
    }

    [Fact]
    public void SingleNonLetterAtEnd_ReturnsFailure()
    {
        // Arrange
        string input = "abc1";

        // Act
        var result = Validate(input);

        // Assert
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Must contain only letters.", result.ErrorMessage);
    }

    [Fact]
    public void UnicodeLetters_ReturnsSuccess()
    {
        // Arrange
        string input = "café"; // Contains 'é', a Unicode letter

        // Act
        var result = Validate(input);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }
}
