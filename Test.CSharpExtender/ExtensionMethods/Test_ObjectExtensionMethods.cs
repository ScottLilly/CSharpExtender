using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_ObjectExtensionMethods
{
    [Fact]
    public void TestDeepClone()
    {
        var original = new TestClass { Value = 1 };
        var clone = original.DeepClone();

        Assert.NotSame(original, clone);
        Assert.Equal(original.Value, clone.Value);
    }

    [Fact]
    public void TestIsNumericType()
    {
        Assert.True(1.IsNumericType());
        Assert.False("test".IsNumericType());
    }

    [Fact]
    public void TestIsIntegerType()
    {
        Assert.True(1.IsIntegerType());
        Assert.False(1.1.IsIntegerType());
    }

    [Fact]
    public void TestIsFloatingPointType()
    {
        Assert.True(1.1.IsFloatingPointType());
        Assert.False(1.IsFloatingPointType());
    }

    [Fact]
    public void TestIsNull()
    {
        object obj = null;
        Assert.True(obj.IsNull());
    }

    [Fact]
    public void TestIsNotNull()
    {
        object obj = new object();
        Assert.True(obj.IsNotNull());
    }

    [Fact]
    public void TestIsOfType()
    {
        object obj = "test";
        Assert.True(obj.IsOfType<string>());
        Assert.False(obj.IsOfType<int>());
    }

    [Fact]
    public void TestIsNotOfType()
    {
        object obj = "test";
        Assert.True(obj.IsNotOfType<int>());
        Assert.False(obj.IsNotOfType<string>());
    }

    [Fact]
    public void TestIsOfTypeOrSubclass()
    {
        object obj = new TestClass();
        Assert.True(obj.IsOfTypeOrSubclass<TestClass>());
        Assert.False(obj.IsOfTypeOrSubclass<string>());
    }

    [Fact]
    public void TestIsNotOfTypeOrSubclass()
    {
        object obj = new TestClass();
        Assert.True(obj.IsNotOfTypeOrSubclass<string>());
        Assert.False(obj.IsNotOfTypeOrSubclass<TestClass>());
    }

    private class TestClass
    {
        public int Value { get; set; }
    }
}
