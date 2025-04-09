using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpExtender.DataAnnotations;

public class AlphaOnlyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        string input = value.ToString();
        
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
