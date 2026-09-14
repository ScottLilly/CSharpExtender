using System;
using System.Collections.Generic;
using System.Xml;

namespace CSharpExtender.Services;

/// <summary>
/// Redacts values in an XmlDocument, for paths matching any of the supplied regex patterns.
/// </summary>
/// <remarks>
/// Paths are the chain of element names from the root, separated by ".", so the "ssn" element in
/// &lt;person&gt;&lt;ssn/&gt;&lt;/person&gt; has the path "person.ssn". An attribute is the
/// element's path plus ".@" and the attribute name, so "person.@id". Repeated sibling elements are
/// not indexed: every element sharing a path is redacted.
/// A redacted element keeps its tag and loses all of its content, including child elements. A
/// redacted attribute keeps its name and gets an empty value.
/// Patterns are matched anywhere in a path rather than against the whole of it, so a pattern that
/// matches an element also matches the paths of everything below it: redacting "person.ssn" also
/// blanks the attributes on that element.
/// </remarks>
public class XmlRedactionService(List<string> redactedPaths, bool ignoreCase = false)
    : BaseRedactionService(redactedPaths, ignoreCase), IRedactionService<XmlDocument>
{
    /// <summary>
    /// Redact values in the given XmlDocument.
    /// </summary>
    /// <param name="obj">The XmlDocument to redact. It is modified in place.</param>
    /// <returns>The redacted XmlDocument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public XmlDocument Redact(XmlDocument obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        RedactDocument(obj);

        return obj;
    }

    /// <summary>
    /// Converts a raw XML string into an XmlDocument, applies redaction, and returns the result.
    /// </summary>
    /// <param name="text">The raw XML string to convert and redact.</param>
    /// <returns>The redacted XmlDocument.</returns>
    /// <exception cref="XmlException">Thrown when <paramref name="text"/> is not valid XML.</exception>
    public XmlDocument Redact(string text)
    {
        var document = new XmlDocument();
        document.LoadXml(text);

        RedactDocument(document);

        return document;
    }

    /// <summary>
    /// Redact an XmlDocument and return the redacted XML as a string.
    /// </summary>
    /// <param name="obj">The XmlDocument to redact. It is modified in place.</param>
    /// <returns>The redacted XML as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public string RedactToString(XmlDocument obj)
    {
        return Redact(obj).OuterXml;
    }

    /// <summary>
    /// Redact a raw XML string by loading it into an XmlDocument and applying redactions.
    /// </summary>
    /// <param name="text">The XML string to redact.</param>
    /// <returns>The redacted XML string.</returns>
    /// <exception cref="XmlException">Thrown when <paramref name="text"/> is not valid XML.</exception>
    public string RedactToString(string text)
    {
        return Redact(text).OuterXml;
    }

    #region Private Methods

    private void RedactDocument(XmlDocument document)
    {
        if (_isEmptyPattern || document.DocumentElement == null)
        {
            return;
        }

        RedactElement(document.DocumentElement, document.DocumentElement.Name);
    }

    /// <summary>
    /// Walks the element tree once, redacting any element or attribute whose path matches.
    /// </summary>
    private void RedactElement(XmlElement element, string currentPath)
    {
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (_redactedPathRegex.IsMatch($"{currentPath}.@{attribute.Name}"))
            {
                attribute.Value = string.Empty;
            }
        }

        if (_redactedPathRegex.IsMatch(currentPath))
        {
            // Removing the content also removes the children, so there is nothing left to walk.
            element.IsEmpty = true;

            return;
        }

        // Copy the child elements first, because redacting one can remove nodes from the collection.
        var childElements = new List<XmlElement>();

        foreach (XmlNode child in element.ChildNodes)
        {
            if (child is XmlElement childElement)
            {
                childElements.Add(childElement);
            }
        }

        foreach (var childElement in childElements)
        {
            RedactElement(childElement, $"{currentPath}.{childElement.Name}");
        }
    }

    #endregion
}
