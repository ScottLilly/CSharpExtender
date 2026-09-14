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
    private static readonly ConcurrentDictionary<string, MethodInfo> _methodCache = new();

    private const BindingFlags MethodSearchFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

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
    /// <param name="parameters">
    /// The parameters to pass to the method. An overloaded method name is resolved
    /// against the runtime types of these arguments. A null argument has no runtime
    /// type, so a call that passes one can only be resolved by argument count.
    /// </param>
    /// <returns>The result of the method invocation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when no method matches the name and supplied arguments.</exception>
    /// <exception cref="InvalidCastException">Thrown when the method's return type is not assignable to <typeparamref name="TResult"/>.</exception>
    public static TResult InvokeMethod<TResult>(object obj, string methodName, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var method = GetCachedMethod(obj.GetType(), methodName, parameters)
            ?? throw new ArgumentException(
                $"Method {methodName} not found, or no overload of it matches the supplied arguments");

        if (!typeof(TResult).IsAssignableFrom(method.ReturnType))
        {
            throw new InvalidCastException($"Method {methodName} does not return {typeof(TResult).Name}");
        }

        return (TResult)method.Invoke(obj, parameters);
    }

    // Get cached MethodInfo for a type, name and argument shape, or resolve and cache if not present
    private static MethodInfo GetCachedMethod(Type type, string methodName, object[] parameters)
    {
        var arguments = parameters ?? Array.Empty<object>();

        // A null argument has no runtime type, so overloads cannot be told apart
        // by it. Those calls fall back to matching on name and argument count.
        Type[] argumentTypes =
            arguments.Length > 0 && arguments.All(p => p != null)
            ? arguments.Select(p => p.GetType()).ToArray()
            : null;

        string cacheKey =
            $"{type.FullName}|{methodName}|{arguments.Length}|" +
            (argumentTypes == null ? "?" : string.Join(",", argumentTypes.Select(t => t.FullName)));

        return _methodCache.GetOrAdd(cacheKey,
            _ => FindMethod(type, methodName, argumentTypes, arguments.Length));
    }

    private static MethodInfo FindMethod(Type type, string methodName, Type[] argumentTypes, int argumentCount)
    {
        if (argumentTypes != null)
        {
            var exactMatch = type.GetMethod(methodName, MethodSearchFlags, null, argumentTypes, null);

            if (exactMatch != null)
            {
                return exactMatch;
            }
        }

        // Either there were no argument types to match on, or no overload takes
        // exactly those types (an int argument to a long parameter, for example).
        var candidates = type.GetMethods(MethodSearchFlags)
            .Where(m => m.Name == methodName)
            .ToArray();

        if (candidates.Length == 1)
        {
            return candidates[0];
        }

        var byArgumentCount = candidates
            .Where(m => m.GetParameters().Length == argumentCount)
            .ToArray();

        // More than one remaining candidate is ambiguous, and there is no way for
        // the caller to say which they meant, so treat it as no match.
        return byArgumentCount.Length == 1 ? byArgumentCount[0] : null;
    }
}