using CSharpExtender.Common;
using CSharpExtender.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for StringBuilders
/// </summary>
public static class StringBuilderExtensionMethods
{
    /// <summary>
    /// Append a string to the StringBuilder if the condition is true
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="condition"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static StringBuilder AppendIf(this StringBuilder sb, Func<bool> condition, string value, StringBuilderOptions options = null)
    {
        ArgumentNullException.ThrowIfNull(condition);

        if (condition())
        {
            sb.Append(ProcessText(value, options));
        }

        return sb;
    }

    /// <summary>
    /// Append a string to the StringBuilder if the condition is true, followed by a new line
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="condition"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static StringBuilder AppendLineIf(this StringBuilder sb, Func<bool> condition, string value, StringBuilderOptions options = null)
    {
        ArgumentNullException.ThrowIfNull(condition);

        if (condition())
        {
            sb.AppendLine(ProcessText(value, options));
        }

        return sb;
    }

    /// <summary>
    /// Append a string to the StringBuilder if the text is not empty or whitespace
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="text"></param>
    /// <param name="options"></param>
    public static void AppendLineIfNotEmpty(this StringBuilder sb, string text, StringBuilderOptions options = null)
    {
        sb.AppendLineIf(() => !string.IsNullOrWhiteSpace(text), text, options);
    }

    /// <summary>
    /// Append a formatted string to the StringBuilder
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="format"></param>
    /// <param name="options"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static StringBuilder AppendFormatted(this StringBuilder sb, string format, StringBuilderOptions options = null, params object[] args)
    {
        sb.Append(ProcessText(string.Format(format, args), options));

        return sb;
    }

    /// <summary>
    /// Append a formatted string to the StringBuilder, followed by a new line
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="format"></param>
    /// <param name="options"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static StringBuilder AppendLineFormatted(this StringBuilder sb, string format, StringBuilderOptions options = null, params object[] args)
    {
        sb.AppendLine(ProcessText(string.Format(format, args), options));

        return sb;
    }

    /// <summary>
    /// Append a collection of items to the StringBuilder, joined by a separator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="separator"></param>
    /// <param name="items"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static StringBuilder AppendJoined<T>(this StringBuilder sb, string separator, IEnumerable<T> items, StringBuilderOptions options = null)
    {
        if (items?.Any() == true)
        {
            sb.Append(ProcessText(string.Join(separator, items), options));
        }

        return sb;
    }

    /// <summary>
    /// Append a collection of items to the StringBuilder, joined by a separator, followed by a new line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="separator"></param>
    /// <param name="items"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static StringBuilder AppendLineJoined<T>(this StringBuilder sb, string separator, IEnumerable<T> items, StringBuilderOptions options = null)
    {
        if (items?.Any() == true)
        {
            sb.AppendLine(ProcessText(string.Join(separator, items), options));
        }

        return sb;
    }

    /// <summary>
    /// Append a string to the StringBuilder
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static StringBuilder Append(this StringBuilder sb, string value, StringBuilderOptions options = null)
    {
        sb.Append(ProcessText(value, options));

        return sb;
    }

    /// <summary>
    /// Append a string to the StringBuilder, followed by a new line
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static StringBuilder AppendLine(this StringBuilder sb, string value, StringBuilderOptions options = null)
    {
        sb.AppendLine(ProcessText(value, options));

        return sb;
    }

    #region Private Methods

    private static string ProcessText(string text, StringBuilderOptions options = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        options ??= new StringBuilderOptions();

        string result = text;

        // Apply max length
        if (options.MaxLength.HasValue && result.Length > options.MaxLength.Value)
        {
            result = result.Substring(0, options.MaxLength.Value);
        }

        // Case transformation
        if (options.ToUpper && !options.ToLower)
        {
            result = result.ToUpper();
        }
        else if (options.ToLower && !options.ToUpper)
        {
            result = result.ToLower();
        }

        // HTML escaping
        if (options.EscapeHtml)
        {
            result = HttpUtility.HtmlEncode(result);
        }

        // Apply format
        if (!string.IsNullOrEmpty(options.Format))
        {
            result = string.Format(options.Format, result);
        }

        // Add prefix and suffix
        if (!string.IsNullOrEmpty(options.PrefixText))
        {
            result = options.PrefixText + result;
        }
        if (!string.IsNullOrEmpty(options.SuffixText))
        {
            result += options.SuffixText;
        }

        // Apply indentation
        if (options.IndentLevel > 0)
        {
            string indent = options.IndentType == IndentType.Tabs
                ? new string('\t', options.IndentLevel)
                : new string(' ', options.IndentLevel * options.IndentDepth);

            result = indent + result;
        }

        return result;
    }

    #endregion
}