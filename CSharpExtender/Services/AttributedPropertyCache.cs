using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace CSharpExtender.Services;

/// <summary>
/// Holds the properties of a type that carry a particular attribute, along with
/// the attribute instance, so the extension methods that apply an attribute do not
/// re-read it on every call.
/// </summary>
/// <typeparam name="TAttribute">The attribute being looked for.</typeparam>
/// <remarks>
/// Held per closed attribute type rather than in one dictionary keyed on both the
/// type and the attribute, so a lookup is a single dictionary hit.
/// </remarks>
internal static class AttributedPropertyCache<TAttribute> where TAttribute : Attribute
{
    private static readonly ConcurrentDictionary<Type, List<(PropertyInfo Property, TAttribute Attribute)>> s_carryingTheAttribute =
        new ConcurrentDictionary<Type, List<(PropertyInfo, TAttribute)>>();

    private static readonly ConcurrentDictionary<(Type, string), (PropertyInfo Property, TAttribute Attribute)> s_byName =
        new ConcurrentDictionary<(Type, string), (PropertyInfo, TAttribute)>();

    /// <summary>
    /// Every property on the type that carries the attribute, in declaration order.
    /// </summary>
    internal static List<(PropertyInfo Property, TAttribute Attribute)> ForType(Type type) =>
        s_carryingTheAttribute.GetOrAdd(type, static t =>
        {
            var carrying = new List<(PropertyInfo, TAttribute)>();

            foreach (var property in PropertyCache.GetProperties(t))
            {
                var attribute = property.GetCustomAttribute<TAttribute>();

                if (attribute != null)
                {
                    carrying.Add((property, attribute));
                }
            }

            return carrying;
        });

    /// <summary>
    /// The named property and its attribute. Property is null when the type does
    /// not have that property, and Attribute is null when it does not carry one.
    /// </summary>
    internal static (PropertyInfo Property, TAttribute Attribute) ForProperty(
        Type type, string propertyName) =>
        s_byName.GetOrAdd((type, propertyName), static key =>
        {
            var property = PropertyCache.GetProperty(key.Item1, key.Item2);

            return (property, property?.GetCustomAttribute<TAttribute>());
        });
}
