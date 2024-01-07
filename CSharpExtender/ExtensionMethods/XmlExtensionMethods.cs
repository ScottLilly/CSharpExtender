using System;
using System.Xml;

namespace CSharpExtender.ExtensionMethods
{
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

            if (int.TryParse(attribute.Value, out int result))
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

            if (bool.TryParse(attribute.Value, out bool result))
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

            if (DateTime.TryParse(attribute.Value, out DateTime result))
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
            return node.SelectSingleNode(elementName)?.InnerText;
        }

        /// <summary>
        /// Returns the inner text of the specified child element as an integer.
        /// </summary>
        /// <param name="node">The XmlNode to retrieve the child element from.</param>
        /// <param name="elementName">The name of the child element.</param>
        /// <returns>The inner text of the child element as an integer, or the default value for integers if the child element does not exist or its inner text cannot be parsed as an integer.</returns>
        public static int ElementAsInt(this XmlNode node, string elementName)
        {
            XmlNode childNode = node.SelectSingleNode(elementName);

            if (childNode != null && int.TryParse(childNode.InnerText, out int result))
            {
                return result;
            }

            return default;
        }
    }
}