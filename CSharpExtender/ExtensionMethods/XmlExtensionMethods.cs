using System;
using System.Xml;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for XML
/// </summary>
public static class XmlExtensionMethods
{
    /// <summary>
    /// Returns the value of the specified attribute as an integer.
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as an integer, or the default value for integers if the attribute does not exist or cannot be parsed as an integer.</returns>
    public static int AttributeAsInt(this XmlNode node, string attributeName)
    {
        XmlAttribute attribute = node.Attributes?[attributeName];

        if (attribute == null)
        {
            return default;
        }

        if (XmlValueParser.TryParseInt(attribute.Value, out int result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the value of the specified attribute as a string.
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as a string, or null if the attribute does not exist.</returns>
    public static string AttributeAsString(this XmlNode node, string attributeName)
    {
        return node.Attributes?[attributeName]?.Value;
    }

    /// <summary>
    /// Returns the value of the specified attribute as a boolean.
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as a boolean, or the default value for booleans if the attribute does not exist or cannot be parsed as a boolean.</returns>
    public static bool AttributeAsBool(this XmlNode node, string attributeName)
    {
        XmlAttribute attribute = node.Attributes?[attributeName];

        if (attribute == null)
        {
            return default;
        }

        if (XmlValueParser.TryParseBoolean(attribute.Value, out bool result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the value of the specified attribute as a DateTime.
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <returns>The value of the attribute as a DateTime, or the default value for DateTime if the attribute does not exist or cannot be parsed as a DateTime.</returns>
    public static DateTime AttributeAsDateTime(this XmlNode node, string attributeName)
    {
        XmlAttribute attribute = node.Attributes?[attributeName];

        if (attribute == null)
        {
            return default;
        }

        if (XmlValueParser.TryParseDateTime(attribute.Value, out DateTime result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Returns the inner text of the specified child element as a string.
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the child element from.</param>
    /// <param name="elementName">The name of the child element.</param>
    /// <returns>The inner text of the child element as a string, or null if the child element does not exist.</returns>
    public static string ElementAsString(this XmlNode node, string elementName)
    {
        return FindChildElement(node, elementName)?.InnerText;
    }

    /// <summary>
    /// Returns the inner text of the specified child element as an integer.
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the child element from.</param>
    /// <param name="elementName">The name of the child element.</param>
    /// <returns>The inner text of the child element as an integer, or the default value for integers if the child element does not exist or its inner text cannot be parsed as an integer.</returns>
    public static int ElementAsInt(this XmlNode node, string elementName)
    {
        XmlNode childNode = FindChildElement(node, elementName);

        if (childNode != null && XmlValueParser.TryParseInt(childNode.InnerText, out int result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Finds the named child element, without going through XPath when it does not
    /// have to.
    /// </summary>
    /// <remarks>
    /// SelectSingleNode parses its argument as an XPath expression on every call,
    /// which costs more than the lookup itself and allocates. A plain element name
    /// can be found by walking the children instead. Anything that is not a plain
    /// name still goes to SelectSingleNode, so an expression a caller passes today
    /// keeps working, including one that is not valid XPath and throws.
    /// </remarks>
    private static XmlNode FindChildElement(XmlNode node, string elementName)
    {
        if (!IsPlainElementName(elementName))
        {
            return node.SelectSingleNode(elementName);
        }

        for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
        {
            // An XPath name with no prefix matches only elements that are in no
            // namespace, so the walk has to pass over namespaced ones to give the
            // same answer SelectSingleNode gave
            if (child.NodeType == XmlNodeType.Element &&
                child.Name == elementName &&
                string.IsNullOrEmpty(child.NamespaceURI))
            {
                return child;
            }
        }

        return null;
    }

    /// <summary>
    /// Whether the text is an element name that means the same thing to XPath as a
    /// walk of the child elements does.
    /// </summary>
    /// <remarks>
    /// Deliberately strict. A prefixed name is left to XPath because it needs a
    /// namespace manager, and a name that XML would reject, such as one starting
    /// with a digit, is left to XPath because it throws there today.
    /// </remarks>
    private static bool IsPlainElementName(string elementName)
    {
        if (string.IsNullOrEmpty(elementName))
        {
            return false;
        }

        char first = elementName[0];

        if (!char.IsLetter(first) && first != '_')
        {
            return false;
        }

        for (int i = 1; i < elementName.Length; i++)
        {
            char character = elementName[i];

            if (!char.IsLetterOrDigit(character) &&
                character != '_' && character != '-' && character != '.')
            {
                return false;
            }
        }

        return true;
    }
}