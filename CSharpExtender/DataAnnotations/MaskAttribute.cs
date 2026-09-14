using System;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Declares how a property's value should be masked when it is displayed or logged.
/// </summary>
/// <remarks>
/// This is not a ValidationAttribute. It does not check the value, it records how to
/// hide it. Apply it with ToMaskedString in MaskExtensionMethods.
/// </remarks>
/// <example>
/// [Mask(MaskChar = '*', VisiblePrefixLength = 4, VisibleSuffixLength = 4)]
/// public string CreditCardNumber { get; set; }   // "1234-****-****-5678"
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class MaskAttribute : Attribute
{
    /// <summary>
    /// Character each masked character is replaced with. Defaults to '*'.
    /// </summary>
    public char MaskChar { get; set; } = '*';

    /// <summary>
    /// How many characters to leave visible at the start of the value.
    /// </summary>
    public int VisiblePrefixLength { get; set; }

    /// <summary>
    /// How many characters to leave visible at the end of the value.
    /// </summary>
    public int VisibleSuffixLength { get; set; }

    /// <summary>
    /// When true, characters that are not letters or digits stay visible and do not
    /// count towards the visible lengths, so a card or phone number keeps its shape.
    /// Defaults to true.
    /// </summary>
    public bool PreserveSeparators { get; set; } = true;
}
