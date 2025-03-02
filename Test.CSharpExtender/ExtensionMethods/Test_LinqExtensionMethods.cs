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
    public void DetectDuplicatePropertyValues()
    {
        var people = new List<Person>()
        { 
            new() { Id = 1, FirstName = "John", LastName = "Smith" }, 
            new() { Id = 2, FirstName = "Jane", LastName = "SMITH" } 
        };

        Assert.False(people.HasDuplicatePropertyValue(p => p.FirstName));

        // Check that the string override of IgnoreCase works
        // True, because the default is case-sensitive
        Assert.False(people.HasDuplicatePropertyValue(p => p.LastName));

        Assert.True(people.HasDuplicatePropertyValue(p => p.LastName, ignoreCase: true));
        Assert.False(people.HasDuplicatePropertyValue(p => p.LastName, ignoreCase: false));
    }

    private class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}