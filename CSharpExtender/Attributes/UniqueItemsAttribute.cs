using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CSharpExtender.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueItemsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // If the value is null, let [Required] handle it if needed
        if (value == null)
        {
            return ValidationResult.Success;
        }

        // Ensure the value is a collection.
        // Need to explicitly check for string, as it is enumerable.
        if (value is string || value is not IEnumerable collection)
        {
            return new ValidationResult("The UniqueItemsAttribute must be applied to a collection.");
        }

        var items = collection.Cast<object>().ToList();
        if (items.Count == 0)
        {
            return ValidationResult.Success;
        }

        // Check for duplicates based on type
        for (int i = 0; i < items.Count - 1; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                if (AreItemsEqual(items[i], items[j]))
                {
                    // Use ErrorMessage if provided; otherwise, use default
                    string errorMessage = ErrorMessage ??
                        $"The list contains duplicate items at indices {i} and {j}.";
                    return new ValidationResult(errorMessage);
                }
            }
        }

        return ValidationResult.Success;
    }

    private bool AreItemsEqual(object item1, object item2)
    {
        if (item1 == null && item2 == null)
        {
            return true;
        }
        if (item1 == null || item2 == null)
        {
            return false;
        }

        // Handle simple types (e.g., string, int)
        if (item1.GetType().IsValueType || item1 is string)
        {
            return item1.Equals(item2);
        }

        // Handle complex objects by comparing all properties
        var properties = item1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var value1 = prop.GetValue(item1);
            var value2 = prop.GetValue(item2);

            if (value1 == null && value2 == null)
            {
                continue;
            }
            if (value1 == null || value2 == null)
            {
                return false;
            }
            if (!value1.Equals(value2))
            {
                return false;
            }
        }

        return true;
    }
}