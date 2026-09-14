using CSharpExtender.Services;
using System;
using System.Reflection;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Provides extension methods for objects to perform reflection operations using the <see cref="SmartReflection"/> class.
/// </summary>
public static class SmartReflectionExtensionMethods
{
    /// <summary>
    /// Checks if the object's type has a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to check for.</typeparam>
    /// <param name="obj">The object to inspect.</param>
    /// <returns>True if the object's type has the specified attribute; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public static bool HasAttribute<TAttribute>(this object obj) where TAttribute : Attribute
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.HasAttribute<TAttribute>(obj.GetType());
    }

    /// <summary>
    /// Checks if a property on the object's type has a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to check for.</typeparam>
    /// <param name="obj">The object to inspect.</param>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns>True if the property has the specified attribute; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    public static bool HasPropertyAttribute<TAttribute>(this object obj, string propertyName) where TAttribute : Attribute
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.HasPropertyAttribute<TAttribute>(obj.GetType(), propertyName);
    }

    /// <summary>
    /// Gets all properties on the object's type that have a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
    /// <param name="obj">The object to inspect.</param>
    /// <returns>An array of <see cref="PropertyInfo"/> for properties with the specified attribute.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public static PropertyInfo[] GetPropertiesWithAttribute<TAttribute>(this object obj) where TAttribute : Attribute
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.GetPropertiesWithAttribute<TAttribute>(obj.GetType());
    }

    /// <summary>
    /// Gets the value of a property from the object.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property value.</typeparam>
    /// <param name="obj">The object to get the property value from.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The property value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    /// <exception cref="InvalidCastException">Thrown when the property type is not assignable to <typeparamref name="TProperty"/>.</exception>
    public static TProperty GetPropertyValue<TProperty>(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.GetPropertyValue<TProperty>(obj, propertyName);
    }

    /// <summary>
    /// Sets the value of a property on the object.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property value.</typeparam>
    /// <param name="obj">The object to set the property value on.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the property is read-only.</exception>
    /// <exception cref="InvalidCastException">Thrown when the property type is not assignable to <typeparamref name="TProperty"/>.</exception>
    public static void SetPropertyValue<TProperty>(this object obj, string propertyName, TProperty value)
    {
        ArgumentNullException.ThrowIfNull(obj);

        SmartReflection.SetPropertyValue(obj, propertyName, value);
    }

    /// <summary>
    /// Gets the names of all public instance properties on the object's type.
    /// </summary>
    /// <param name="obj">The object to inspect.</param>
    /// <returns>An array of property names.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public static string[] GetPropertyNames(this object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.GetPropertyNames(obj.GetType());
    }

    /// <summary>
    /// Checks if the object's type has a specific property.
    /// </summary>
    /// <param name="obj">The object to inspect.</param>
    /// <param name="propertyName">The name of the property to check for.</param>
    /// <returns>True if the property exists; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public static bool HasProperty(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.HasProperty(obj.GetType(), propertyName);
    }

    /// <summary>
    /// Gets the type of a property on the object's type.
    /// </summary>
    /// <param name="obj">The object to inspect.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The <see cref="Type"/> of the property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the property does not exist.</exception>
    public static Type GetPropertyType(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.GetPropertyType(obj.GetType(), propertyName);
    }

    /// <summary>
    /// Invokes a method on the object by name.
    /// </summary>
    /// <typeparam name="TResult">The expected return type of the method.</typeparam>
    /// <param name="obj">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>The result of the method invocation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the method does not exist.</exception>
    /// <exception cref="InvalidCastException">Thrown when the method's return type is not assignable to <typeparamref name="TResult"/>.</exception>
    public static TResult InvokeMethod<TResult>(this object obj, string methodName, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return SmartReflection.InvokeMethod<TResult>(obj, methodName, parameters);
    }
}