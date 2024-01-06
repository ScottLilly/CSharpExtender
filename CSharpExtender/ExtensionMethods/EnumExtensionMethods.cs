using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Extension methods for Enums
    /// </summary>
    public static class EnumExtensionMethods
    {
        // Cache the display names for each enum value
        private static readonly ConcurrentDictionary<Enum, string> _enumDescriptions = 
            new ConcurrentDictionary<Enum, string>();

        /// <summary>
        /// Gets the description of an enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The value of the DescriptionAttribute of the enum value.</returns>
        public static string GetEnumDescription(this Enum value)
        {
            if (!_enumDescriptions.TryGetValue(value, out string description))
            {
                description = value.GetType()
                    .GetField(value.ToString())?
                    .GetCustomAttribute<DescriptionAttribute>()?.Description
                    ?? value.ToString();

                _enumDescriptions.TryAdd(value, description);
            }

            return description;
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
}