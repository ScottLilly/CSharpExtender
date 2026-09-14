using CSharpExtender.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods that apply the MaskAttribute declared on a property
/// </summary>
public static class MaskExtensionMethods
{
    /// <summary>
    /// Returns the named property's value with its MaskAttribute applied.
    /// </summary>
    /// <param name="obj">The object to read the property from.</param>
    /// <param name="propertyName">The name of the property to mask.</param>
    /// <returns>
    /// The masked value. A property with no MaskAttribute is returned unmasked, and
    /// a null value returns an empty string.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if obj is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the property does not exist.</exception>
    public static string ToMaskedString(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var property = obj.GetType().GetProperty(propertyName)
            ?? throw new ArgumentException($"Property {propertyName} not found");

        return MaskValue(property.GetValue(obj), property.GetCustomAttribute<MaskAttribute>());
    }

    /// <summary>
    /// Returns every property carrying a MaskAttribute, with the mask applied,
    /// keyed by property name.
    /// </summary>
    /// <param name="obj">The object to read.</param>
    /// <returns>A dictionary of property name to masked value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if obj is null.</exception>
    public static IDictionary<string, string> ToMaskedStrings(this object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var masked = new Dictionary<string, string>();

        foreach (var property in obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attribute = property.GetCustomAttribute<MaskAttribute>();

            if (attribute == null)
            {
                continue;
            }

            masked[property.Name] = MaskValue(property.GetValue(obj), attribute);
        }

        return masked;
    }

    private static string MaskValue(object value, MaskAttribute attribute)
    {
        string text = value?.ToString() ?? string.Empty;

        return attribute == null
            ? text
            : text.Mask(attribute.MaskChar, attribute.VisiblePrefixLength,
                attribute.VisibleSuffixLength, attribute.PreserveSeparators);
    }
}
