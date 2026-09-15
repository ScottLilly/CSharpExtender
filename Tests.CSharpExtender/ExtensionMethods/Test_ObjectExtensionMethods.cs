using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

public class Test_ObjectExtensionMethods
{
    [Fact]
    public void TestDeepClone()
    {
        var original = new TestClass { Value = 1 };
        var clone = original.DeepClone()!;

        Assert.NotSame(original, clone);
        Assert.Equal(original.Value, clone.Value);
    }

    [Fact]
    public void DeepClone_NestedObject_IsAlsoCloned()
    {
        var original = new NestedOwner { Child = new TestClass { Value = 7 } };

        var clone = original.DeepClone()!;

        Assert.NotSame(original, clone);
        Assert.NotSame(original.Child, clone.Child);
        Assert.Equal(7, clone.Child.Value);
    }

    [Fact]
    public void DeepClone_CircularReference_DoesNotThrow()
    {
        // The shared options keep ReferenceHandler.Preserve, which is the only
        // reason this terminates
        var parent = new CircularNode { Name = "parent" };
        var child = new CircularNode { Name = "child", Parent = parent };
        parent.Child = child;

        var clone = parent.DeepClone()!;

        Assert.NotSame(parent, clone);
        Assert.Equal("parent", clone.Name);
        Assert.Equal("child", clone.Child.Name);
        Assert.Same(clone, clone.Child.Parent);
    }

    [Fact]
    public void DeepClone_RepeatedCalls_AreIndependent()
    {
        // The options instance is now shared across calls, so prove one clone
        // cannot affect the next
        var original = new TestClass { Value = 1 };

        var first = original.DeepClone()!;
        first.Value = 99;
        var second = original.DeepClone()!;

        Assert.Equal(1, second.Value);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void DeepClone_Null_ReturnsNull()
    {
        TestClass nullObject = null;

        Assert.Null(nullObject.DeepClone());
    }

    private class NestedOwner
    {
        public TestClass Child { get; set; }
    }

    private class CircularNode
    {
        public string Name { get; set; }
        public CircularNode Child { get; set; }
        public CircularNode Parent { get; set; }
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
        object? obj = null;
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

    [Fact]
    public void TestIsOfType_SubclassInstance_IsNotTheBaseType()
    {
        object obj = new DerivedTestClass();

        Assert.True(obj.IsOfType<DerivedTestClass>());
        Assert.False(obj.IsOfType<TestClass>());
        Assert.True(obj.IsNotOfType<TestClass>());
    }

    [Fact]
    public void TestIsOfType_GenericAndTypeParameterOverloadsAgree()
    {
        object obj = new DerivedTestClass();

        Assert.Equal(obj.IsOfType<TestClass>(), obj.IsOfType(typeof(TestClass)));
        Assert.Equal(obj.IsOfType<DerivedTestClass>(), obj.IsOfType(typeof(DerivedTestClass)));
    }

    [Fact]
    public void TestIsOfTypeOrSubclass_SubclassInstance_IsTheBaseType()
    {
        object obj = new DerivedTestClass();

        Assert.True(obj.IsOfTypeOrSubclass<DerivedTestClass>());
        Assert.True(obj.IsOfTypeOrSubclass<TestClass>());
        Assert.False(obj.IsNotOfTypeOrSubclass<TestClass>());
    }

    [Fact]
    public void TestIsOfTypeOrSubclass_Interface_IsMatchedByImplementation()
    {
        object obj = new TestClass();

        Assert.True(obj.IsOfTypeOrSubclass<ITestMarker>());
        Assert.True(obj.IsOfTypeOrSubclass(typeof(ITestMarker)));
        Assert.False(obj.IsOfType<ITestMarker>());
    }

    [Fact]
    public void TestIsOfTypeOrSubclass_OnType_IncludesTheTypeItself()
    {
        Assert.True(typeof(TestClass).IsOfTypeOrSubclass<TestClass>());
        Assert.True(typeof(TestClass).IsOfTypeOrSubclass(typeof(TestClass)));
        Assert.False(typeof(TestClass).IsNotOfTypeOrSubclass<TestClass>());
    }

    [Fact]
    public void TestIsOfTypeOrSubclass_OnType_IncludesSubclassesAndInterfaces()
    {
        Assert.True(typeof(DerivedTestClass).IsOfTypeOrSubclass<TestClass>());
        Assert.True(typeof(TestClass).IsOfTypeOrSubclass<ITestMarker>());
        Assert.False(typeof(string).IsOfTypeOrSubclass<TestClass>());
    }

    [Fact]
    public void TestIsOfType_OnType_IsExactMatchOnly()
    {
        Assert.True(typeof(TestClass).IsOfType<TestClass>());
        Assert.False(typeof(DerivedTestClass).IsOfType<TestClass>());
        Assert.True(typeof(DerivedTestClass).IsNotOfType<TestClass>());
    }

    [Fact]
    public void TestIsOfType_ObjectAndTypeReceiversAgree()
    {
        object obj = new DerivedTestClass();

        Assert.Equal(obj.IsOfType<TestClass>(), typeof(DerivedTestClass).IsOfType<TestClass>());
        Assert.Equal(obj.IsOfTypeOrSubclass<TestClass>(), typeof(DerivedTestClass).IsOfTypeOrSubclass<TestClass>());
    }

    private interface ITestMarker
    {
    }

    private class TestClass : ITestMarker
    {
        public int Value { get; set; }
    }

    private class DerivedTestClass : TestClass
    {
    }
}
