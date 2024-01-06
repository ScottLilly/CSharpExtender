using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_LinqExtensionMethods
{
    [Fact]
    public void None_EmptyList_ReturnsTrue()
    {
        var list = new List<int>();
        Assert.True(list.None());
        Assert.True(list.None(x => x > 5));
    }

    [Fact]
    public void None_WithCondition_ReturnsExpectedResult()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        Assert.True(list.None(x => x > 5));
        Assert.False(list.None(x => x < 5));
    }

    [Fact]
    public void None_WithoutCondition_ReturnsExpectedResult()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        Assert.False(list.None());
    }

    [Fact]
    public void ForEach_IEnumerableAction_AppliesAction()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        int sum = 0;
        list.ForEach(x => sum += x);
        Assert.Equal(15, sum);
    }

    [Fact]
    public void ForEach_ListAction_AppliesAction()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        int sum = 0;
        list.ForEach(x => sum += x);
        Assert.Equal(15, sum);
    }

    [Fact]
    public void RandomElement_ReturnsElementInList()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        var randomElement = list.RandomElement();
        Assert.Contains(randomElement, list);
    }

    [Fact]
    public void RandomElement_EmptyList_ReturnsDefault()
    {
        var list = new List<int>();
        var randomElement = list.RandomElement();
        Assert.Equal(default, randomElement);
    }
}