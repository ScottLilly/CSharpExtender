using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for Enums
/// </summary>
public static class EnumExtensionMethods
{
    // Cache the description for each enum value. Held per closed TEnum rather
    // than in one dictionary keyed on Type, so a lookup neither boxes the value
    // nor needs a second dictionary hit to find the right inner cache.
    private static class DescriptionCache<TEnum> where TEnum : Enum
    {
        internal static readonly ConcurrentDictionary<TEnum, string> Values =
            new ConcurrentDictionary<TEnum, string>();
    }

    /// <summary>
    /// Gets the description of an enum value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>
    /// The value of the DescriptionAttribute of the enum value, or the value's
    /// string representation when it has no DescriptionAttribute, is a combination
    /// of [Flags] members, or is not a defined member of the enum.
    /// </returns>
    public static string GetEnumDescription<TEnum>(this TEnum value) where TEnum : Enum
    {
        return DescriptionCache<TEnum>.Values.GetOrAdd(value, static enumValue =>
        {
            // GetField returns null when the value is not a single named member,
            // which is the case for undefined values and combined [Flags] values.
            var fieldInfo = typeof(TEnum).GetField(enumValue.ToString());

            if (fieldInfo == null)
            {
                return enumValue.ToString();
            }

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? attributes[0].Description
                : enumValue.ToString();
        });
    }

    // Held per closed TEnum for the same reason DescriptionCache is, and kept
    // separate from it because the two attributes are read independently: a member
    // can carry either, both, or neither.
    private static class DisplayNameCache<TEnum> where TEnum : Enum
    {
        internal static readonly ConcurrentDictionary<TEnum, string> Values =
            new ConcurrentDictionary<TEnum, string>();
    }

    /// <summary>
    /// Gets the display name of an enum value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>
    /// The name from the DisplayAttribute of the enum value, or the value's string
    /// representation when it has no DisplayAttribute, its DisplayAttribute does not
    /// set a Name, is a combination of [Flags] members, or is not a defined member of
    /// the enum.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the DisplayAttribute sets a ResourceType that does not hold a public
    /// static string property of the Name it was given. That is a misconfigured
    /// attribute rather than a missing display name.
    /// </exception>
    public static string GetEnumDisplayName<TEnum>(this TEnum value) where TEnum : Enum
    {
        return DisplayNameCache<TEnum>.Values.GetOrAdd(value, static enumValue =>
        {
            // GetField returns null when the value is not a single named member,
            // which is the case for undefined values and combined [Flags] values.
            var fieldInfo = typeof(TEnum).GetField(enumValue.ToString());

            if (fieldInfo == null)
            {
                return enumValue.ToString();
            }

            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length == 0)
            {
                return enumValue.ToString();
            }

            // GetName rather than Name, because it is what resolves a name through a
            // ResourceType. It returns null when the attribute sets other properties
            // but not Name, which falls back the same way a missing attribute does.
            return attributes[0].GetName() ?? enumValue.ToString();
        });
    }

    // Held per closed enum type, because an enum's members are fixed when it is
    // compiled. Read-only because every caller is handed this same instance, and an
    // array could be cast back to T[] through the IEnumerable<T> and written to,
    // which would change what every later caller sees.
    private static class ValueCache<TEnum> where TEnum : Enum
    {
        internal static readonly ReadOnlyCollection<TEnum> Values =
            Array.AsReadOnly((TEnum[])Enum.GetValues(typeof(TEnum)));
    }

    /// <summary>
    /// Gets all values of a specific enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>
    /// A read-only collection of all values of the enum type. The same instance is
    /// returned to every caller, so it cannot be added to, removed from, or written
    /// through.
    /// </returns>
    public static IEnumerable<T> GetEnumValues<T>() where T : Enum
    {
        return ValueCache<T>.Values;
    }

    /// <summary>
    /// Gets all descriptions of a specific enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>An IEnumerable of all descriptions of the enum type.</returns>
    public static IEnumerable<string> GetEnumDescriptions<T>() where T : Enum
    {
        foreach (var value in GetEnumValues<T>())
        {
            yield return value.GetEnumDescription();
        }
    }

    /// <summary>
    /// Gets all display names of a specific enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>An IEnumerable of all display names of the enum type.</returns>
    public static IEnumerable<string> GetEnumDisplayNames<T>() where T : Enum
    {
        foreach (var value in GetEnumValues<T>())
        {
            yield return value.GetEnumDisplayName();
        }
    }

    /// <summary>
    /// Parses a string to an enum value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing.</param>
    /// <returns>The parsed enum value.</returns>
    public static T ParseEnum<T>(string value, bool ignoreCase = true) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }
}