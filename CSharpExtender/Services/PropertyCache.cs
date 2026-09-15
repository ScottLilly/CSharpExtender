using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace CSharpExtender.Services;

/// <summary>
/// Holds each type's public instance properties, so repeated lookups do not
/// re-enter <see cref="Type.GetProperties()"/>.
/// </summary>
/// <remarks>
/// A type's properties cannot change while the process runs, so the answer is the
/// same every time it is worked out. One cache serves the whole library.
/// </remarks>
internal static class PropertyCache
{
    private const BindingFlags _searchFlags = BindingFlags.Public | BindingFlags.Instance;

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> s_properties =
        new ConcurrentDictionary<Type, PropertyInfo[]>();

    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo> s_byName =
        new ConcurrentDictionary<(Type, string), PropertyInfo>();

    internal static PropertyInfo[] GetProperties(Type type) =>
        s_properties.GetOrAdd(type, static t => t.GetProperties(_searchFlags));

    /// <summary>
    /// The named property, or null when the type does not have one. Callers decide
    /// what a missing property means.
    /// </summary>
    /// <remarks>
    /// When a derived type hides a base property with "new", the type has two
    /// properties of that name and the first one found wins. Which of the two that
    /// is depends on the order Type.GetProperties returns them in, and .NET does
    /// not guarantee one.
    /// </remarks>
    internal static PropertyInfo GetProperty(Type type, string propertyName) =>
        s_byName.GetOrAdd((type, propertyName), static key =>
        {
            var properties = GetProperties(key.Item1);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == key.Item2)
                {
                    return properties[i];
                }
            }

            return null;
        });
}
