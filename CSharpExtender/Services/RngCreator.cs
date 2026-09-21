using System.Security.Cryptography;

namespace CSharpExtender.Services;

/// <summary>
/// Class to create cryptographically random numbers
/// </summary>
public static class RngCreator
{
    /// <summary>
    /// A cryptographically random number in the range, with both ends included.
    /// </summary>
    /// <param name="minimumValue">Smallest value that can be returned.</param>
    /// <param name="maximumValue">Largest value that can be returned.</param>
    /// <returns>A random number from minimumValue through maximumValue.</returns>
    public static int GetNumberBetween(int minimumValue, int maximumValue)
    {
        return RandomNumberGenerator.GetInt32(minimumValue, maximumValue + 1);
    }
}