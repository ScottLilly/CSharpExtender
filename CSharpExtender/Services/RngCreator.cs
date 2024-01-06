using System;
using System.Security.Cryptography;

namespace CSharpExtender.Services
{
    public static class RngCreator
    {
        public static int GetNumberBetween(int minimumValue, int maximumValue)
        {
            // Using RNGCryptoServiceProvider for cryptographic-quality random numbers
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);
                int value = BitConverter.ToInt32(randomNumber, 0);

                // Convert the generated value to be within the specified range
                return (Math.Abs(value) % (maximumValue - minimumValue + 1)) + minimumValue;
            }
        }
    }
}
