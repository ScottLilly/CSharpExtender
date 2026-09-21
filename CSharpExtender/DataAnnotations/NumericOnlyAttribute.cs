using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Validates that a string property or field holds a number and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// A null or empty value passes, so that [Required] decides whether a value has to be
/// present at all. This matches <see cref="AlphaOnlyAttribute"/>.
/// </para>
/// <para>
/// Every option is off by default, so the default accepts digits and nothing else. Each
/// one turns on one more thing the text may hold. Leading and trailing whitespace is
/// never accepted.
/// </para>
/// <para>
/// With every option off, each character has to be an ASCII digit, "0" through "9". No
/// culture takes part in that, and a run of digits passes however long it is. This is
/// stricter than <see cref="ExtensionMethods.StringExtensionMethods.IsDigitsOnly"/>, which
/// uses <see cref="char.IsDigit(char)"/> and so accepts any Unicode decimal digit. ASCII
/// keeps the default a subset of what the options below accept, because the number parser
/// they use reads ASCII digits only.
/// </para>
/// <para>
/// With any option on, the text is read with
/// <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)"/>
/// under <see cref="CultureInfo.CurrentCulture"/>, or under the invariant culture when
/// <see cref="UseInvariantCulture"/> is set, so which separators a value may use is the
/// culture's answer rather than this attribute's. "1.234,56" is a number in de-DE and is
/// not one in en-US, and a culture whose group separator is neither a comma nor a period,
/// such as fr-FR with its no-break space, is read its own way.
/// </para>
/// <para>
/// The two are different questions, and they disagree at the top of the range. A value
/// too large for <see cref="decimal"/> cannot be parsed, so a run of more than 29 digits
/// fails with an option on and passes with none on. "Every character is a digit" has no
/// ceiling; "this reads as a number" has decimal's.
/// </para>
/// <para>
/// This is for strings, and it throws <see cref="InvalidCastException"/> when it is given
/// a member of any other type. A numeric member cannot hold text that is not a number: a
/// UI bound to one rejects the entry while converting it, before the property is ever
/// set, so there would be nothing left for this attribute to catch. Worse, the text a
/// numeric member converts to carries the culture's separators, so a decimal property
/// would fail this attribute's own default.
/// </para>
/// </remarks>
/// <example>
/// [NumericOnly(AllowDecimal = true, AllowCurrencySymbol = true)]
/// public string Price { get; set; }
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class NumericOnlyAttribute : ValidationAttribute
{
    /// <summary>
    /// Accept a decimal separator, the "." in "9.99" under en-US.
    /// </summary>
    public bool AllowDecimal { get; set; }

    /// <summary>
    /// Accept group separators, the "," in "1,234,567" under en-US.
    /// </summary>
    public bool AllowThousandsSeparator { get; set; }

    /// <summary>
    /// Accept the culture's currency symbol, the "$" in "$9.99" under en-US. The culture
    /// also decides which position the symbol may take, and the separators become the
    /// culture's currency separators rather than its number separators.
    /// </summary>
    public bool AllowCurrencySymbol { get; set; }

    /// <summary>
    /// Accept a leading sign, the "-" in "-42". A leading "+" is a sign as well, so it is
    /// accepted too. A trailing sign is not, in the cultures that write one.
    /// </summary>
    public bool AllowNegative { get; set; }

    /// <summary>
    /// Read the value under <see cref="CultureInfo.InvariantCulture"/> instead of
    /// <see cref="CultureInfo.CurrentCulture"/>. Set this where the text is a machine
    /// value that means the same thing everywhere, rather than something a person typed.
    /// </summary>
    public bool UseInvariantCulture { get; set; }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Whether a value has to be present is [Required]'s business, not this attribute's
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not string input)
        {
            throw new InvalidCastException(
                $"The {validationContext.MemberName ?? "member"} of type {value.GetType().Name} " +
                $"must be a string. {nameof(NumericOnlyAttribute)} validates strings only.");
        }

        if (input.Length == 0)
        {
            return ValidationResult.Success;
        }

        NumberStyles styles = AllowedStyles();

        // With nothing turned on there is no separator to place and no sign to read, so
        // this is a question about characters rather than one about numbers. Asking it
        // that way keeps the culture out of it, and lets a run of digits be any length
        bool isValid = styles == NumberStyles.None
            ? IsAsciiDigitsOnly(input)
            : decimal.TryParse(input, styles, FormatProvider(), out _);

        return isValid
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? "Must contain only numbers.");
    }

    private static bool IsAsciiDigitsOnly(string input)
    {
        ReadOnlySpan<char> span = input.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsAsciiDigit(span[i]))
            {
                return false;
            }
        }

        return true;
    }

    private NumberStyles AllowedStyles()
    {
        NumberStyles styles = NumberStyles.None;

        if (AllowDecimal)
        {
            styles |= NumberStyles.AllowDecimalPoint;
        }

        if (AllowThousandsSeparator)
        {
            styles |= NumberStyles.AllowThousands;
        }

        if (AllowCurrencySymbol)
        {
            styles |= NumberStyles.AllowCurrencySymbol;
        }

        if (AllowNegative)
        {
            styles |= NumberStyles.AllowLeadingSign;
        }

        return styles;
    }

    private IFormatProvider FormatProvider() =>
        UseInvariantCulture ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
}
