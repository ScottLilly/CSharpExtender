using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for numbers
/// </summary>
public static class NumericExtensionMethods
{
    private const float _minimumAbsoluteTolerance = 1e-7f;

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

    /// <summary>
    /// Determines whether the current float value is approximately equal to another float value,
    /// within a specified percentage tolerance or a minimum absolute threshold.
    /// </summary>
    /// <param name="value">The float value to compare from (this instance).</param>
    /// <param name="other">The float value to compare to.</param>
    /// <param name="tolerancePercentage">
    /// The percentage of the maximum absolute value between the two numbers that is allowed as a difference.
    /// Defaults to 0.0001 (0.01%). A minimum absolute tolerance is also applied for very small values.
    /// </param>
    /// <returns>
    /// <c>true</c> if the absolute difference between the two values is less than or equal to the calculated tolerance; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method handles <c>NaN</c> and <c>Infinity</c> values explicitly:
    /// returns <c>false</c> if either value is <c>NaN</c>, and compares directly if either is <c>Infinity</c>.
    /// When both values are near zero, a minimum absolute tolerance of 1e-7 is used to ensure meaningful comparison.
    /// </remarks>
    public static bool ApproximatelyEquals(this float value, float other, float tolerancePercentage = 0.0001f)
    {
        if (float.IsNaN(value) || float.IsNaN(other))
        {
            return false;
        }

        if (float.IsInfinity(value) || float.IsInfinity(other))
        {
            return value == other;
        }

        float max = Math.Max(Math.Abs(value), Math.Abs(other));
        float tolerance = Math.Max(max * tolerancePercentage, _minimumAbsoluteTolerance);

        return Math.Abs(value - other) <= tolerance;
    }

    /// <summary>
    /// Calculates the sample standard deviation, dividing by n-1. Use this when the
    /// values are a sample drawn from a larger population, which is the usual case.
    /// </summary>
    /// <param name="values">The values to measure. Needs at least two.</param>
    /// <returns>The sample standard deviation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
    /// <exception cref="ArgumentException">Thrown if there are fewer than two values.</exception>
    public static double StandardDeviation(this IEnumerable<double> values) =>
        CalculateStandardDeviation(values, isSample: true);

    /// <summary>
    /// Calculates the sample standard deviation, dividing by n-1. Use this when the
    /// values are a sample drawn from a larger population, which is the usual case.
    /// </summary>
    /// <param name="values">The values to measure. Needs at least two.</param>
    /// <returns>The sample standard deviation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
    /// <exception cref="ArgumentException">Thrown if there are fewer than two values.</exception>
    public static double StandardDeviation(this IEnumerable<int> values) =>
        CalculateStandardDeviation(AsDoubles(values), isSample: true);

    /// <summary>
    /// Calculates the sample standard deviation, dividing by n-1. Use this when the
    /// values are a sample drawn from a larger population, which is the usual case.
    /// </summary>
    /// <param name="values">The values to measure. Needs at least two.</param>
    /// <returns>The sample standard deviation, as a double. Decimal has no square root.</returns>
    /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
    /// <exception cref="ArgumentException">Thrown if there are fewer than two values.</exception>
    public static double StandardDeviation(this IEnumerable<decimal> values) =>
        CalculateStandardDeviation(AsDoubles(values), isSample: true);

    /// <summary>
    /// Calculates the population standard deviation, dividing by n. Use this when the
    /// values are the complete set, not a sample of it.
    /// </summary>
    /// <param name="values">The values to measure. Needs at least one.</param>
    /// <returns>The population standard deviation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
    /// <exception cref="ArgumentException">Thrown if there are no values.</exception>
    public static double PopulationStandardDeviation(this IEnumerable<double> values) =>
        CalculateStandardDeviation(values, isSample: false);

    /// <summary>
    /// Calculates the population standard deviation, dividing by n. Use this when the
    /// values are the complete set, not a sample of it.
    /// </summary>
    /// <param name="values">The values to measure. Needs at least one.</param>
    /// <returns>The population standard deviation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
    /// <exception cref="ArgumentException">Thrown if there are no values.</exception>
    public static double PopulationStandardDeviation(this IEnumerable<int> values) =>
        CalculateStandardDeviation(AsDoubles(values), isSample: false);

    /// <summary>
    /// Calculates the population standard deviation, dividing by n. Use this when the
    /// values are the complete set, not a sample of it.
    /// </summary>
    /// <param name="values">The values to measure. Needs at least one.</param>
    /// <returns>The population standard deviation, as a double. Decimal has no square root.</returns>
    /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
    /// <exception cref="ArgumentException">Thrown if there are no values.</exception>
    public static double PopulationStandardDeviation(this IEnumerable<decimal> values) =>
        CalculateStandardDeviation(AsDoubles(values), isSample: false);

    private static IEnumerable<double> AsDoubles<T>(IEnumerable<T> values) where T : struct =>
        values?.Select(v => Convert.ToDouble(v));

    private static double CalculateStandardDeviation(IEnumerable<double> values, bool isSample)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        // Materialize once, so a lazy source is not enumerated twice
        var list = values as IReadOnlyList<double> ?? values.ToList();

        if (list.Count < (isSample ? 2 : 1))
        {
            throw new ArgumentException(
                isSample
                ? "A sample standard deviation needs at least two values."
                : "A population standard deviation needs at least one value.",
                nameof(values));
        }

        double total = 0;

        for (int i = 0; i < list.Count; i++)
        {
            total += list[i];
        }

        double mean = total / list.Count;
        double sumOfSquaredDeviations = 0;

        for (int i = 0; i < list.Count; i++)
        {
            double deviation = list[i] - mean;
            sumOfSquaredDeviations += deviation * deviation;
        }

        return Math.Sqrt(sumOfSquaredDeviations / (isSample ? list.Count - 1 : list.Count));
    }
}