using System.Xml;

namespace CSharpExtender.ExtensionMethods;

public static class XmlExtensionMethods
{
    public static int AttributeAsInt(this XmlNode node, string attributeName)
    {
        return Convert.ToInt32(node.AttributeAsString(attributeName));
    }

    public static string AttributeAsString(this XmlNode node, string attributeName)
    {
        XmlAttribute attribute = node.Attributes?[attributeName];

        if (attribute == null)
        {
            throw new ArgumentException($"The attribute '{attributeName}' does not exist");
        }

        return attribute.Value;
    }
}