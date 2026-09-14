using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace CSharpExtender.Services;

/// <summary>
/// Provides efficient, type-safe reflection utilities with property caching for improved performance.
/// </summary>
public static class SmartReflection
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    // Get cached PropertyInfo for a type, or fetch and cache if not present
    private static PropertyInfo[] GetCachedProperties(Type type)
    {
        return _propertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
    }

    /// <summary>
    /// Checks if a type has a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to check for.</typeparam>
    /// <param name="type">The type to inspect.</param>
    /// <returns>True if the type has the specified attribute; otherwise, false.</returns>
    public static bool HasAttribute<TAttribute>(Type type) where TAttribute : Attribute
    {
        return type.GetCustomAttribute<TAttribute>() != null;
    }

    /// <summary>
    /// Checks if a property on a type has a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to check for.</typeparam>
    /// <param name="type">The type containing the property.</param>
    /// <param name="propertyName">The name of the property to inspect.</param>
    /// <returns>True if the property has the specified attribute; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    public static bool HasPropertyAttribute<TAttribute>(Type type, string propertyName) where TAttribute : Attribute
    {
        var prop = GetCachedProperties(type).FirstOrDefault(p => p.Name == propertyName)
            ?? throw new ArgumentException($"Property {propertyName} not found");

        return prop?.GetCustomAttribute<TAttribute>() != null;
    }

    /// <summary>
    /// Gets all properties on a type that have a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An array of <see cref="PropertyInfo"/> for properties with the specified attribute.</returns>
    public static PropertyInfo[] GetPropertiesWithAttribute<TAttribute>(Type type) where TAttribute : Attribute
    {
        return GetCachedProperties(type)
            .Where(p => p.GetCustomAttribute<TAttribute>() != null)
            .ToArray();
    }

    /// <summary>
    /// Gets the value of a property from an object.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property value.</typeparam>
    /// <param name="obj">The object to get the property value from.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The property value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    /// <exception cref="InvalidCastException">Thrown when the property type is not assignable to <typeparamref name="TProperty"/>.</exception>
    public static TProperty GetPropertyValue<TProperty>(object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var prop = GetCachedProperties(obj.GetType()).FirstOrDefault(p => p.Name == propertyName)
            ?? throw new ArgumentException($"Property {propertyName} not found");

        if (!typeof(TProperty).IsAssignableFrom(prop.PropertyType))
        {
            throw new InvalidCastException($"Property {propertyName} is not of type {typeof(TProperty).Name}");
        }

        return (TProperty)prop.GetValue(obj);
    }

    /// <summary>
    /// Sets the value of a property on an object.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property value.</typeparam>
    /// <param name="obj">The object to set the property value on.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the property is read-only.</exception>
    /// <exception cref="InvalidCastException">Thrown when the property type is not assignable to <typeparamref name="TProperty"/>.</exception>
    public static void SetPropertyValue<TProperty>(object obj, string propertyName, TProperty value)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var prop = GetCachedProperties(obj.GetType()).FirstOrDefault(p => p.Name == propertyName)
            ?? throw new ArgumentException($"Property {propertyName} not found");

        if (!prop.CanWrite)
        {
            throw new InvalidOperationException($"Property {propertyName} is read-only");
        }

        if (!typeof(TProperty).IsAssignableFrom(prop.PropertyType))
        {
            throw new InvalidCastException($"Property {propertyName} cannot accept type {typeof(TProperty).Name}");
        }

        prop.SetValue(obj, value);
    }

    /// <summary>
    /// Gets the names of all public instance properties on a type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An array of property names.</returns>
    public static string[] GetPropertyNames(Type type)
    {
        return GetCachedProperties(type).Select(p => p.Name).ToArray();
    }

    /// <summary>
    /// Checks if a type has a specific property.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="propertyName">The name of the property to check for.</param>
    /// <returns>True if the property exists; otherwise, false.</returns>
    public static bool HasProperty(Type type, string propertyName)
    {
        return GetCachedProperties(type).Any(p => p.Name == propertyName);
    }

    /// <summary>
    /// Gets the type of a property on a type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The <see cref="Type"/> of the property.</returns>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    public static Type GetPropertyType(Type type, string propertyName)
    {
        var prop = GetCachedProperties(type).FirstOrDefault(p => p.Name == propertyName)
            ?? throw new ArgumentException($"Property {propertyName} not found");

        return prop.PropertyType;
    }

    /// <summary>
    /// Invokes a method on an object by name.
    /// </summary>
    /// <typeparam name="TResult">The expected return type of the method.</typeparam>
    /// <param name="obj">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>The result of the method invocation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the method does not exist.</exception>
    /// <exception cref="InvalidCastException">Thrown when the method's return type is not assignable to <typeparamref name="TResult"/>.</exception>
    public static TResult InvokeMethod<TResult>(object obj, string methodName, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var method =
            obj.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new ArgumentException($"Method {methodName} not found");

        if (!typeof(TResult).IsAssignableFrom(method.ReturnType))
        {
            throw new InvalidCastException($"Method {methodName} does not return {typeof(TResult).Name}");
        }

        return (TResult)method.Invoke(obj, parameters);
    }
}