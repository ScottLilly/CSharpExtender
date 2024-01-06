namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Scott's extension methods for numbers
    /// </summary>
    public static class NumericExtensionMethods
    {
        public static bool IsEven(this int val)
        {
            return val % 2 == 0;
        }

        public static bool IsOdd(this int val)
        {
            return !val.IsEven();
        }
    }
}