using CSharpExtender.Collections;

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

    [Fact]
    public void Test_TryGet_Found()
    {
        var cache = new GenericCache<int, string>();
        cache.Set(42, "answer");

        var result = cache.TryGet(42, out var value);

        Assert.True(result);
        Assert.Equal("answer", value);
    }

    [Fact]
    public void Test_TryGet_NotFound()
    {
        var cache = new GenericCache<int, string>();

        var result = cache.TryGet(99, out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void Test_TryGet_Expired()
    {
        var cache = new GenericCache<int, string>();
        cache.Set(1, "temp", TimeSpan.FromMilliseconds(10));

        Thread.Sleep(50); // wait for expiration

        var result = cache.TryGet(1, out var value);

        Assert.False(result);
        Assert.Null(value);
    }
}