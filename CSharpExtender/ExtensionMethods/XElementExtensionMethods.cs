using System;
using System.Xml.Linq;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for XElement, mirroring the XmlNode set in
/// <see cref="XmlExtensionMethods"/> member for member.
/// </summary>
/// <remarks>
/// Every member answers the way its XmlNode twin does: a missing attribute or element
/// returns null for the string methods and the type's default for the others, and a value
/// that will not parse returns the type's default rather than throwing. An unprefixed name
/// matches only attributes and elements that are in no namespace, which is also what the
/// XmlNode set does.
/// The one difference is a null receiver. These throw <see cref="ArgumentNullException"/>,
/// where the XmlNode set throws <see cref="NullReferenceException"/> from dereferencing the
/// node it was handed.
/// </remarks>
public static class XElementExtensionMethods
{
    /// <summary>
    /// Returns the value of the specified attribute as a string.
    /// </summary>
    /// <param name="element">The XElement to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as a string, or null if the attribute does not exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static string? AttributeAsString(this XElement element, string attributeName)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(attributeName);

        return element.Attribute(attributeName)?.Value;
    }

    /// <summary>
    /// Returns the value of the specified attribute as an integer.
    /// </summary>
    /// <param name="element">The XElement to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as an integer, or the default value for integers if the attribute does not exist or cannot be parsed as an integer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static int AttributeAsInt(this XElement element, string attributeName)
    {
        string? value = element.AttributeAsString(attributeName);

        if (value != null && XmlValueParser.TryParseInt(value, out int result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the value of the specified attribute as a boolean.
    /// </summary>
    /// <param name="element">The XElement to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as a boolean, or the default value for booleans if the attribute does not exist or cannot be parsed as a boolean.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static bool AttributeAsBool(this XElement element, string attributeName)
    {
        string? value = element.AttributeAsString(attributeName);

        if (value != null && XmlValueParser.TryParseBoolean(value, out bool result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the value of the specified attribute as a DateTime.
    /// </summary>
    /// <param name="element">The XElement to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as a DateTime, or the default value for DateTime if the attribute does not exist or cannot be parsed as a DateTime.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static DateTime AttributeAsDateTime(this XElement element, string attributeName)
    {
        string? value = element.AttributeAsString(attributeName);

        if (value != null && XmlValueParser.TryParseDateTime(value, out DateTime result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the text of the specified child element as a string.
    /// </summary>
    /// <param name="element">The XElement to retrieve the child element from.</param>
    /// <param name="elementName">The name of the child element.</param>
    /// <returns>The text of the child element as a string, or null if the child element does not exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static string? ElementAsString(this XElement element, string elementName)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(elementName);

        // XElement.Value concatenates the text of everything below the element, which is
        // what XmlNode.InnerText does, so the two sets answer the same for a child that
        // has children of its own
        return element.Element(elementName)?.Value;
    }

    /// <summary>
    /// Returns the text of the specified child element as an integer.
    /// </summary>
    /// <param name="element">The XElement to retrieve the child element from.</param>
    /// <param name="elementName">The name of the child element.</param>
    /// <returns>The text of the child element as an integer, or the default value for integers if the child element does not exist or its text cannot be parsed as an integer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static int ElementAsInt(this XElement element, string elementName)
    {
        string? value = element.ElementAsString(elementName);

        if (value != null && XmlValueParser.TryParseInt(value, out int result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the value of an attribute or a child element of the given name, whichever
    /// carries it, for XML that writes the same value either way.
    /// </summary>
    /// <param name="element">The XElement to read from.</param>
    /// <param name="name">The name of the attribute or child element.</param>
    /// <returns>
    /// The attribute's value if there is one, otherwise the child element's text, otherwise
    /// null. The attribute wins when both carry the name.
    /// </returns>
    /// <remarks>
    /// This is the one member with no XmlNode counterpart. It looks at the same places its
    /// two halves do, so it finds a direct child rather than any descendant, and it returns
    /// null for a miss rather than an empty string, the way every other member here does.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
    public static string? GetValue(this XElement element, string name)
    {
        return element.AttributeAsString(name) ?? element.ElementAsString(name);
    }
}
