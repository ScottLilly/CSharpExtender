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
    public int? MaxLength { get; set; }
    public string PrefixText { get; set; }
    public string SuffixText { get; set; }
    public bool ToUpper { get; set; }
    public bool ToLower { get; set; }
    public string Format { get; set; }
}
