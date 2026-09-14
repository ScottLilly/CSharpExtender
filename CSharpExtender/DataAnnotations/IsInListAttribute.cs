using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Validates that a property or field holds one of a fixed list of allowed values.
/// </summary>
/// <remarks>
/// A null value passes, so that [Required] decides whether a value has to be
/// present at all.
/// </remarks>
/// <example>
/// [IsInList("Red", "Green", "Blue", IgnoreCase = true)]
/// public string Color { get; set; }
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class IsInListAttribute : ValidationAttribute
{
    private readonly object[] _allowedValues;

    /// <summary>
    /// Compare string values without regard to case. Has no effect on other types.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// Creates the attribute with the values the member is allowed to hold.
    /// </summary>
    /// <param name="allowedValues">The allowed values.</param>
    public IsInListAttribute(params object[] allowedValues)
    {
        _allowedValues = allowedValues ?? Array.Empty<object>();
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Whether a value has to be present is [Required]'s business, not this attribute's
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (_allowedValues.Length == 0)
        {
            return new ValidationResult(
                "The IsInListAttribute must be given at least one allowed value.");
        }

        if (_allowedValues.Any(allowed =>
                AnnotationValueComparer.AreEqual(value, allowed, IgnoreCase)))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage ??
            $"Must be one of: {string.Join(", ", _allowedValues)}.");
    }
}
