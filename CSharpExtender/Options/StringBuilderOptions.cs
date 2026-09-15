using CSharpExtender.Common;

namespace CSharpExtender.Options;

/// <summary>
/// Options for StringBuilder append operations
/// </summary>
public class StringBuilderOptions
{
    public bool EscapeHtml { get; set; }
    public int IndentLevel { get; set; }
    public IndentType IndentType { get; set; } = IndentType.Spaces;
    public int IndentDepth { get; set; } = 4;
    /// <summary>
    /// Caps the length of the value, not the length of the finished string.
    /// </summary>
    /// <remarks>
    /// The cap is applied after <see cref="ToUpper"/>, <see cref="ToLower"/> and
    /// <see cref="EscapeHtml"/> have rewritten the value, and before <see cref="Format"/>,
    /// <see cref="PrefixText"/>, <see cref="SuffixText"/> and the indent wrap it. Those four
    /// are the caller's own text rather than the value, so they are not counted and are never
    /// cut in half: a <see cref="MaxLength"/> of 3 with a <see cref="SuffixText"/> of "]"
    /// returns four characters.
    /// </remarks>
    public int? MaxLength { get; set; }
    public string PrefixText { get; set; }
    public string SuffixText { get; set; }
    public bool ToUpper { get; set; }
    public bool ToLower { get; set; }
    public string Format { get; set; }
}
