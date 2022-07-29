using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_LinqExtensionMethods
{
    [Fact]
    public void Test_None()
    {
        var items = new List<string> { "a", "asd", "ASD" };

        Assert.True(items.None(i => i.Contains('z')));
        Assert.False(items.None(i => i.Contains('d')));
    }

    [Fact]
    public void Test_None_EmptyList()
    {
        var itemsEmpty = new List<string>();
        var itemsFull = new List<int>() { 1, 2, 5 };

        Assert.True(itemsEmpty.None());
        Assert.False(itemsFull.None());
    }
}