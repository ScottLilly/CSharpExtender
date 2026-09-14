using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for Enums
/// </summary>
public static class EnumExtensionMethods
{
    // Cache the display names for each enum value. Held per closed TEnum rather
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

    /// <summary>
    /// Gets all values of a specific enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>An IEnumerable of all values of the enum type.</returns>
    public static IEnumerable<T> GetEnumValues<T>() where T : Enum
    {
        return (T[])Enum.GetValues(typeof(T));
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