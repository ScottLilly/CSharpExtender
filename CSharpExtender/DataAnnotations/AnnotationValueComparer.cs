using System;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Compares a value supplied to an attribute against a value read off an object
/// at runtime.
/// </summary>
/// <remarks>
/// Attribute arguments have to be compile-time constants, and a whole-number
/// literal in one is an int whatever the member it decorates is declared as. A
/// plain Equals would therefore report [IsInList(1, 2)] on a long property as
/// never matching, so numeric values are compared by value rather than by their
/// boxed type.
/// </remarks>
internal static class AnnotationValueComparer
{
    internal static bool AreEqual(object? runtimeValue, object? attributeValue, bool ignoreCase)
    {
        if (runtimeValue == null || attributeValue == null)
        {
            return runtimeValue == null && attributeValue == null;
        }

        if (runtimeValue is string runtimeText && attributeValue is string attributeText)
        {
            return string.Equals(runtimeText, attributeText,
                ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.Ordinal);
        }

        if (runtimeValue.Equals(attributeValue))
        {
            return true;
        }

        return AreNumericallyEqual(runtimeValue, attributeValue);
    }

    private static bool AreNumericallyEqual(object left, object right)
    {
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            return false;
        }

        try
        {
            return Convert.ToDecimal(left) == Convert.ToDecimal(right);
        }
        catch (OverflowException)
        {
            // A double outside decimal's range cannot equal any attribute constant
            return false;
        }
    }

    private static bool IsNumeric(object value) =>
        value is byte or sbyte or short or ushort or int or uint
            or long or ulong or float or double or decimal;
}
