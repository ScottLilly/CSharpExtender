using System.Text;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for StringBuilders
/// </summary>
public static class StringBuilderExtensionMethods
{
    /// <summary>
    /// Append a line to the StringBuilder if the text is not null or empty.
    /// This prevents the StringBuilder from having empty lines.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="text">Text to add as an appended line, if not null, empty, or whitespace.</param>
    public static void AppendLineIfNotEmpty(this StringBuilder sb, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            sb.AppendLine(text);
        }
    }
}