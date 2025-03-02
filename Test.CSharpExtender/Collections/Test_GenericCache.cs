namespace Test.CSharpExtender.Collections;

public class Test_GenericCache
{
    [Fact]
    public void Test_CacheRetrieval_Found()
    {
        var cache = new GenericCache<int, string>();
        cache.Set(1, "one");
        cache.Set(2, "two");

        Assert.Equal("one", cache.Get(1));
        Assert.Equal("two", cache.Get(2));
    }

    [Fact]
    public void Test_CacheRetrieval_NotFound()
    {
        var cache = new GenericCache<int, string>();
        cache.Set(1, "one");
        cache.Set(2, "two");

        Assert.Null(cache.Get(3));
    }

    [Fact]
    public void Test_CacheRetrieval_Remove()
    {
        var cache = new GenericCache<int, string>();
        cache.Set(1, "one");
        cache.Set(2, "two");

        Assert.Equal("one", cache.Get(1));

        cache.Remove(1);

        Assert.Null(cache.Get(1));
    }
}
