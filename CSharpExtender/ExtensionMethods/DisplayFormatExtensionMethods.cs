using CSharpExtender.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods that apply the DisplayFormatAttribute declared on a property.
/// </summary>
/// <remarks>
/// DisplayFormatAttribute is the one in System.ComponentModel.DataAnnotations. It is
/// normally only honored by UI frameworks, so these methods let the same declaration
/// be used in plain code.
/// </remarks>
public static class DisplayFormatExtensionMethods
{
    /// <summary>
    /// Returns the named property's value formatted with its DisplayFormatAttribute.
    /// </summary>
    /// <param name="obj">The object to read the property from.</param>
    /// <param name="propertyName">The name of the property to format.</param>
    /// <param name="formatProvider">The culture to format with. Defaults to the current culture.</param>
    /// <returns>
    /// The formatted value. A property with no DisplayFormatAttribute, or one with no
    /// DataFormatString, falls back to the value's own ToString.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if obj is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the property does not exist.</exception>
    public static string ToDisplayString(this object obj, string propertyName,
        IFormatProvider? formatProvider = null)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var entry = AttributedPropertyCache<DisplayFormatAttribute>
            .ForProperty(obj.GetType(), propertyName);

        if (entry.Property == null)
        {
            throw new ArgumentException($"Property {propertyName} not found");
        }

        return FormatValue(entry.Property.GetValue(obj), entry.Attribute, formatProvider);
    }

    /// <summary>
    /// Returns every property carrying a DisplayFormatAttribute, formatted, keyed by
    /// property name.
    /// </summary>
    /// <param name="obj">The object to read.</param>
    /// <param name="formatProvider">The culture to format with. Defaults to the current culture.</param>
    /// <returns>A dictionary of property name to formatted value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if obj is null.</exception>
    public static IDictionary<string, string> ToDisplayStrings(this object obj,
        IFormatProvider? formatProvider = null)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var formatted = new Dictionary<string, string>();

        foreach (var entry in
            AttributedPropertyCache<DisplayFormatAttribute>.ForType(obj.GetType()))
        {
            formatted[entry.Property.Name] =
                FormatValue(entry.Property.GetValue(obj), entry.Attribute, formatProvider);
        }

        return formatted;
    }

    private static string FormatValue(object? value, DisplayFormatAttribute? attribute,
        IFormatProvider? formatProvider)
    {
        var culture = formatProvider ?? CultureInfo.CurrentCulture;

        if (value == null)
        {
            return attribute?.NullDisplayText ?? string.Empty;
        }

        string? formatString = attribute?.DataFormatString;

        if (string.IsNullOrEmpty(formatString))
        {
            return Convert.ToString(value, culture) ?? string.Empty;
        }

        // DataFormatString conventionally wraps the specifier, as in "{0:yyyy-MM-dd}",
        // but a bare specifier is the more natural thing to write, so accept both
        if (formatString.Contains("{0"))
        {
            return string.Format(culture, formatString, value);
        }

        return value is IFormattable formattable
            ? formattable.ToString(formatString, culture)
            : Convert.ToString(value, culture) ?? string.Empty;
    }
}
