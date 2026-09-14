using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Validates that a collection property or field contains no duplicate items.
/// </summary>
/// <remarks>
/// Two items are duplicates when they are the same type and every public instance
/// property is equal. The comparison is shallow: a property whose value is a
/// reference type without its own Equals override compares by reference.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueItemsAttribute : ValidationAttribute
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> s_propertyCache =
        new ConcurrentDictionary<Type, PropertyInfo[]>();

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

    private static bool AreItemsEqual(object item1, object item2)
    {
        if (item1 == null && item2 == null)
        {
            return true;
        }
        if (item1 == null || item2 == null)
        {
            return false;
        }

        // Items of different types are never duplicates. This also keeps the
        // property walk below from reading item1's PropertyInfo off an item2
        // that does not have that property, which throws TargetException.
        var itemType = item1.GetType();
        if (itemType != item2.GetType())
        {
            return false;
        }

        // Handle simple types (e.g., string, int)
        if (itemType.IsValueType || item1 is string)
        {
            return item1.Equals(item2);
        }

        // Handle complex objects by comparing all properties
        var properties = s_propertyCache.GetOrAdd(itemType,
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

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