using System;
using System.Security.Cryptography;

namespace CSharpExtender.Services;

/// <summary>
/// Class to create cryptographically random numbers
/// </summary>
public static class RngCreator
{
    public static int GetNumberBetween(int minimumValue, int maximumValue)
    {
        return RandomNumberGenerator.GetInt32(minimumValue, maximumValue + 1);
    }
}