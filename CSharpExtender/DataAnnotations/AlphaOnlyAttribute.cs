using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Validates that a value contains only letters.
/// </summary>
/// <remarks>
/// <para>
/// A null or empty value passes, so that [Required] decides whether a value has to be
/// present at all.
/// </para>
/// <para>
/// A letter is anything <see cref="char.IsLetter(char)"/> accepts, so letters outside
/// the ASCII range pass, and a space does not.
/// </para>
/// <para>
/// Unlike <see cref="NumericOnlyAttribute"/>, which validates strings only, this checks
/// whatever <see cref="object.ToString"/> returns for a member of any type. Put it on a
/// string.
/// </para>
/// </remarks>
public class AlphaOnlyAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        string? input = value.ToString();
        
        if (string.IsNullOrEmpty(input))
        {
            return ValidationResult.Success;
        }

        ReadOnlySpan<char> span = input.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsLetter(span[i]))
            {
                return new ValidationResult(ErrorMessage ?? "Must contain only letters.");
            }
        }

        return ValidationResult.Success;
    }
}
