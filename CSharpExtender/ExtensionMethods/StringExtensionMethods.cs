namespace CSharpExtender.ExtensionMethods;

public static class StringExtensionMethods
{
    public static bool Matches(this string text, string matchingText)
    {
        return text.Equals(matchingText, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool DoesNotMatch(this string text, string comparisonText)
    {
        return !text.Matches(comparisonText);
    }
}