using System.Collections.Generic;
using System;
using System.Linq;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for LINQ
/// </summary>
public static class LinqExtensionMethods
{
    /// <summary>
    /// Checks if none of the elements in the collection satisfy the provided condition.
    /// If no condition is provided, it checks if the collection is empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="elements">The collection to check.</param>
    /// <param name="func">The condition to check each element against.</param>
    /// <returns>True if none of the elements satisfy the condition, false otherwise.</returns>
    public static bool None<T>(this IEnumerable<T> elements, Func<T, bool> func = null)
    {
        if (func == null)
        {
            return !elements.Any();
        }

        return !elements.Any(func);
    }

    /// <summary>
    /// Applies an action to each element in the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="elements">The collection to iterate over.</param>
    /// <param name="action">The action to apply to each element.</param>
    public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
    {
        foreach (T element in elements)
        {
            action(element);
        }
    }

    /// <summary>
    /// Checks if any element in the collection has a duplicate property value.
    /// Ignores default values.
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="source">List of items</param>
    /// <param name="propertySelector">Function to get the property value</param>
    /// <param name="ignoreCase">Boolean to specify whether to ignore case</param>
    /// <returns>Whether or not any element in the collection has a duplicate property value</returns>
    public static bool HasDuplicatePropertyValue<T, TProperty>(
        this IEnumerable<T> source,
        Func<T, TProperty> propertySelector,
        IEqualityComparer<TProperty> comparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertySelector);

        comparer ??= EqualityComparer<TProperty>.Default;

        // Store the property values in a HashSet, to quickly identify duplicates
        var propertyValues = new HashSet<TProperty>(comparer);

        foreach (var item in source)
        {
            var value = propertySelector(item);

            if (value != null && !EqualityComparer<TProperty>.Default.Equals(value, default))
            {
                if (!propertyValues.Add(value))
                {
                    return true; // Duplicate found
                }
            }
        }

        return false; // No duplicates found
    }

    /// <summary>
    /// Checks if any element in the collection has a duplicate property value.
    /// This overloaded method allows you to specify the ignore case option.
    /// Default is to be case-sensitive.
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="source">List of items</param>
    /// <param name="propertySelector">Function to get the property value</param>
    /// <param name="ignoreCase">Boolean to specify whether to ignore case</param>
    /// <returns>Whether or not any element in the collection has a duplicate property value</returns>
    public static bool HasDuplicatePropertyValue<T>(
        this IEnumerable<T> source,
        Func<T, string> propertySelector,
        bool ignoreCase = false)
    {
        return HasDuplicatePropertyValue(source, propertySelector,
            ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    }
}