using System.Security.Cryptography;

namespace CSharpExtender.Services;

public static class RngCreator
{
    /// <summary>
    /// Generate a random integer between two values
    /// </summary>
    /// <param name="minimumValue">Lowest possible value (inclusive)</param>
    /// <param name="maximumValue">Highest possible value (inclusive)</param>
    /// <returns></returns>
    public static int GetNumberBetween(int minimumValue, int maximumValue)
    {
        // Need to add one to maximumValue, because otherwise,
        // this function will never generate a value that matches the maximumValue.
        // For example: to get a value from 1 to 10 (inclusive),
        // The code must (effectively) call: RandomNumberGenerator.GetInt32(1, 11);
        return RandomNumberGenerator.GetInt32(minimumValue, maximumValue + 1);
    }
}