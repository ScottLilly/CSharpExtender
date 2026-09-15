using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
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

        var items = Materialize(collection);
        if (items.Count == 0)
        {
            return ValidationResult.Success;
        }

        // Answers "are these all different" in one pass, for the shapes it can
        // judge. It cannot say where a duplicate is, so a false answer still
        // needs the walk below to name the two indexes.
        if (UniqueItemsHashCheck.IsProvablyUnique(items))
        {
            return ValidationResult.Success;
        }

        var comparer = new UniqueItemsComparer(items);

        // Check for duplicates based on type
        for (int i = 0; i < items.Count - 1; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                if (comparer.AreItemsEqual(i, j))
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

    /// <summary>
    /// Copies the collection into a list that can be indexed, at its final size
    /// when the collection knows how big it is.
    /// </summary>
    /// <remarks>
    /// The non-generic ICollection is what a List, an array, and anything else with
    /// a known size implements, so the list is built at its final size and never
    /// has to grow.
    /// </remarks>
    private static List<object?> Materialize(IEnumerable collection)
    {
        var items = collection is ICollection sized
            ? new List<object?>(sized.Count)
            : new List<object?>();

        foreach (object? item in collection)
        {
            items.Add(item);
        }

        return items;
    }
}
