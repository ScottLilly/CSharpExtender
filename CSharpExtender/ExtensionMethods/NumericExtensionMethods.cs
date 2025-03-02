namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for numbers
/// </summary>
public static class NumericExtensionMethods
{
    /// <summary>
    /// Checks if the given integer is even.
    /// </summary>
    /// <param name="val">The integer to check.</param>
    /// <returns>True if the integer is even, false otherwise.</returns>
    public static bool IsEven(this int val)
    {
        return val % 2 == 0;
    }

    /// <summary>
    /// Checks if the given integer is odd.
    /// </summary>
    /// <param name="val">The integer to check.</param>
    /// <returns>True if the integer is odd, false otherwise.</returns>
    public static bool IsOdd(this int val)
    {
        return val % 2 != 0;
    }

    /// <summary>
    /// Checks if the given integer is positive.
    /// </summary>
    /// <param name="val">The integer to check.</param>
    /// <returns>True if the integer is positive, false otherwise.</returns>
    public static bool IsPositive(this int val)
    {
        return val > 0;
    }

    /// <summary>
    /// Checks if the given integer is negative.
    /// </summary>
    /// <param name="val">The integer to check.</param>
    /// <returns>True if the integer is negative, false otherwise.</returns>
    public static bool IsNegative(this int val)
    {
        return val < 0;
    }

    /// <summary>
    /// Checks if the given integer is evenly divisible by another integer.
    /// </summary>
    /// <param name="val">The integer to check.</param>
    /// <param name="divisor">The integer to divide by.</param>
    /// <returns>True if the integer is evenly divisible by the divisor, false otherwise.</returns>
    public static bool IsEvenlyDivisibleBy(this int val, int divisor)
    {
        return val % divisor == 0;
    }
}