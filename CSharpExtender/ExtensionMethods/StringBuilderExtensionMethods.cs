using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CSharpExtender.Options;

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
    public static StringBuilder AppendIf(this StringBuilder sb, Func<bool> condition, string? value, StringBuilderOptions? options = null)
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
    public static StringBuilder AppendLineIf(this StringBuilder sb, Func<bool> condition, string? value, StringBuilderOptions? options = null)
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
    public static void AppendLineIfNotEmpty(this StringBuilder sb, string? text, StringBuilderOptions? options = null)
    {
        // Tested directly, so a call allocates no closure and no delegate
        if (!string.IsNullOrWhiteSpace(text))
        {
            sb.AppendLine(ProcessText(text, options));
        }
    }

    /// <summary>
    /// Append a formatted string to the StringBuilder
    /// </summary>
    /// <param name="sb">StringBuilder object</param>
    /// <param name="format"></param>
    /// <param name="options"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static StringBuilder AppendFormatted(this StringBuilder sb, string format, StringBuilderOptions? options = null, params object[] args)
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
    public static StringBuilder AppendLineFormatted(this StringBuilder sb, string format, StringBuilderOptions? options = null, params object[] args)
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
    public static StringBuilder AppendJoined<T>(this StringBuilder sb, string separator, IEnumerable<T> items, StringBuilderOptions? options = null)
    {
        if (HasItems(ref items))
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
    public static StringBuilder AppendLineJoined<T>(this StringBuilder sb, string separator, IEnumerable<T> items, StringBuilderOptions? options = null)
    {
        if (HasItems(ref items))
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
    public static StringBuilder Append(this StringBuilder sb, string? value, StringBuilderOptions? options = null)
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
    public static StringBuilder AppendLine(this StringBuilder sb, string? value, StringBuilderOptions? options = null)
    {
        sb.AppendLine(ProcessText(value, options));

        return sb;
    }

    #region Private Methods

    // Stands in for the options a caller did not pass. Private, never handed to a
    // caller, and never written to, so one shared instance is safe.
    private static readonly StringBuilderOptions s_defaultOptions = new StringBuilderOptions();

    // string.Join walks the items itself, so this has to answer without walking
    // them a second time. A source whose count can be read without walking it is
    // left alone; anything else is materialized, so the caller's sequence is
    // enumerated exactly once.
    private static bool HasItems<T>(ref IEnumerable<T> items)
    {
        if (items == null)
        {
            return false;
        }

        if (items.TryGetNonEnumeratedCount(out int count))
        {
            return count > 0;
        }

        var materialized = items.ToList();

        items = materialized;

        return materialized.Count > 0;
    }

    private static string? ProcessText(string? text, StringBuilderOptions? options = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        options ??= s_defaultOptions;

        string result = text;

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

        // MaxLength caps the value, so it runs after the options that rewrite the value
        // and before the ones that wrap it. Capping any later would cut the closing half
        // off Format, SuffixText or both.
        if (options.MaxLength.HasValue && result.Length > options.MaxLength.Value)
        {
            result = result.Substring(0, options.MaxLength.Value);
        }

        // Apply format
        if (!string.IsNullOrEmpty(options.Format))
        {
            result = string.Format(options.Format, result);
        }

        // Indentation, prefix and suffix were three separate concatenations, each
        // building another string, plus a fourth for the indent itself. They only
        // wrap what is already there, so they go on in one pass.
        int indentWidth = options.IndentLevel <= 0
            ? 0
            : options.IndentType == IndentType.Tabs
                ? options.IndentLevel
                : options.IndentLevel * options.IndentDepth;

        string prefix = options.PrefixText ?? string.Empty;
        string suffix = options.SuffixText ?? string.Empty;

        if (indentWidth == 0 && prefix.Length == 0 && suffix.Length == 0)
        {
            return result;
        }

        char indentCharacter = options.IndentType == IndentType.Tabs ? '\t' : ' ';

        return string.Create(
            indentWidth + prefix.Length + result.Length + suffix.Length,
            (result, prefix, suffix, indentWidth, indentCharacter),
            static (destination, state) =>
            {
                int position = 0;

                for (int i = 0; i < state.indentWidth; i++)
                {
                    destination[position++] = state.indentCharacter;
                }

                state.prefix.AsSpan().CopyTo(destination.Slice(position));
                position += state.prefix.Length;

                state.result.AsSpan().CopyTo(destination.Slice(position));
                position += state.result.Length;

                state.suffix.AsSpan().CopyTo(destination.Slice(position));
            });
    }

    #endregion
}