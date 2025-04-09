using CSharpExtender.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Test.CSharpExtender.Attributes;

public class UniqueItemsAttributeTests
{
    private static ValidationContext GetValidationContext(object instance) => new(instance);

    // Test class for complex object validation
    private class TestItem(int id, string name)
    {
        public int Id { get; set; } = id;
        public string Name { get; set; } = name;
    }

    [Fact]
    public void UniqueItemsAttribute_NullList_ReturnsSuccess()
    {
        var model = new { Items = null as List<string> };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_EmptyList_ReturnsSuccess()
    {
        var model = new { Items = new List<string>() };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_UniqueStrings_ReturnsSuccess()
    {
        var model = new { Items = new List<string> { "apple", "banana", "cherry" } };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateStrings_ReturnsError()
    {
        var model = new { Items = new List<string> { "apple", "apple", "cherry" } };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("duplicate items", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_UniqueNumbers_ReturnsSuccess()
    {
        var model = new { Items = new List<int> { 1, 2, 3 } };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateNumbers_ReturnsError()
    {
        var model = new { Items = new List<int> { 1, 2, 2 } };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("duplicate items", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_UniqueComplexObjects_ReturnsSuccess()
    {
        var model = new
        {
            Items = new List<TestItem>
            {
                new(1, "apple"),
                new(2, "banana")
            }
        };

        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));
        
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateComplexObjects_ReturnsError()
    {
        var model = new
        {
            Items = new List<TestItem>
            {
                new(1, "apple"),
                new(1, "apple")
            }
        };

        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("duplicate items", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_NonCollection_ThrowsError()
    {
        var model = new { Items = "not a list" };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("must be applied to a collection", result.ErrorMessage);
    }
}