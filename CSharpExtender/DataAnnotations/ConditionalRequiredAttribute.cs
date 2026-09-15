using CSharpExtender.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Validates that a property or field has a value when another property on the
/// same object holds a particular value.
/// </summary>
/// <remarks>
/// The dependent property is read off <see cref="ValidationContext.ObjectInstance"/>,
/// so this only works through a call that supplies the object being validated,
/// such as Validator.ValidateObject or Validator.TryValidateObject.
/// </remarks>
/// <example>
/// [ConditionalRequired(DependentProperty = "IsActive", RequiredWhenValue = true)]
/// public string ActiveUserDetails { get; set; }
/// public bool IsActive { get; set; }
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ConditionalRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// The name of the property whose value decides whether this member is required.
    /// </summary>
    public string? DependentProperty { get; set; }

    /// <summary>
    /// The value of DependentProperty that makes this member required.
    /// </summary>
    public object? RequiredWhenValue { get; set; }

    /// <summary>
    /// Count a string of whitespace as a value. False by default, matching [Required].
    /// </summary>
    public bool AllowEmptyStrings { get; set; }

    /// <summary>
    /// Compare a string DependentProperty value without regard to case.
    /// </summary>
    public bool IgnoreCase { get; set; }

    // Each instance is its own TypeId, so two of these on one member are not
    // collapsed into one by code that de-duplicates attributes
    public override object TypeId => this;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(DependentProperty))
        {
            return new ValidationResult(
                "The ConditionalRequiredAttribute must be given a DependentProperty name.");
        }

        // Null when reached through the single-argument IsValid, which carries no
        // object to read the dependent property from
        if (validationContext?.ObjectInstance == null)
        {
            return new ValidationResult(
                "The ConditionalRequiredAttribute needs a ValidationContext carrying the object being validated.");
        }

        object instance = validationContext.ObjectInstance;
        var dependentProperty = PropertyCache.GetProperty(instance.GetType(), DependentProperty);

        if (dependentProperty == null)
        {
            return new ValidationResult(
                $"Property {DependentProperty} was not found on {instance.GetType().Name}.");
        }

        object? dependentValue = dependentProperty.GetValue(instance);

        // The condition is not met, so this member is not required
        if (!AnnotationValueComparer.AreEqual(dependentValue, RequiredWhenValue, IgnoreCase))
        {
            return ValidationResult.Success;
        }

        if (HasValue(value))
        {
            return ValidationResult.Success;
        }

        string? memberName = validationContext.MemberName;
        string displayName = validationContext.DisplayName ?? memberName ?? "The value";

        string message = ErrorMessage ??
            $"{displayName} is required when {DependentProperty} is {RequiredWhenValue ?? "null"}.";

        return memberName == null
            ? new ValidationResult(message)
            : new ValidationResult(message, new[] { memberName });
    }

    private bool HasValue(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (value is string text)
        {
            return AllowEmptyStrings || !string.IsNullOrWhiteSpace(text);
        }

        return true;
    }
}
