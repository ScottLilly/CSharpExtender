using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Validates that a string property or field contains only letters.
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
/// This is for strings, as <see cref="NumericOnlyAttribute"/> is, and it throws
/// <see cref="InvalidCastException"/> when it is given a member of any other type. A
/// member that is not a string cannot hold text that its own type cannot hold, so there
/// is nothing here to catch, and what would be checked is whatever
/// <see cref="object.ToString"/> prints, which is a formatting decision rather than the
/// value: an enum member passes on the name it happens to be spelled with, a
/// <see cref="Guid"/> never passes at all, and a <see cref="DateTime"/> answers
/// differently depending on the machine's culture.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AlphaOnlyAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Whether a value has to be present is [Required]'s business, not this attribute's
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not string input)
        {
            throw new InvalidCastException(
                $"The {validationContext.MemberName ?? "member"} of type {value.GetType().Name} " +
                $"must be a string. {nameof(AlphaOnlyAttribute)} validates strings only.");
        }

        if (input.Length == 0)
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
