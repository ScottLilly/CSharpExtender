namespace CSharpExtender.Options;

/// <summary>
/// Options for StringBuilder append operations
/// </summary>
public class StringBuilderOptions
{
    /// <summary>
    /// HTML-encodes the text.
    /// </summary>
    public bool EscapeHtml { get; set; }

    /// <summary>
    /// How many levels to indent the text.
    /// </summary>
    public int IndentLevel { get; set; }

    /// <summary>
    /// Whether an indent level is made of tabs or spaces. Defaults to spaces.
    /// </summary>
    public IndentType IndentType { get; set; } = IndentType.Spaces;

    /// <summary>
    /// How many tabs or spaces make up one indent level. Defaults to 4.
    /// </summary>
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

    /// <summary>
    /// Text placed before the value.
    /// </summary>
    public string? PrefixText { get; set; }

    /// <summary>
    /// Text placed after the value.
    /// </summary>
    public string? SuffixText { get; set; }

    /// <summary>
    /// Uppercases the text. Setting this and <see cref="ToLower"/> together leaves the
    /// case alone.
    /// </summary>
    public bool ToUpper { get; set; }

    /// <summary>
    /// Lowercases the text. Setting this and <see cref="ToUpper"/> together leaves the
    /// case alone.
    /// </summary>
    public bool ToLower { get; set; }

    /// <summary>
    /// A composite format string the text is passed through, such as "[{0}]".
    /// </summary>
    public string? Format { get; set; }
}
