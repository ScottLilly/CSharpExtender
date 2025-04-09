using CSharpExtender.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Test.CSharpExtender.DataAnnotations;

public class Test_UniqueItemsAttribute
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
    public void UniqueItemsAttribute_DuplicateStrings_DefaultErrorMessage()
    {
        var model = new { Items = new List<string> { "apple", "apple", "cherry" } };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("duplicate items", result.ErrorMessage);
        Assert.Contains("indices 0 and 1", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateStrings_CustomErrorMessage()
    {
        var model = new { Items = new List<string> { "apple", "apple", "cherry" } };
        var attribute = new UniqueItemsAttribute { ErrorMessage = "All items must be unique in the list." };
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("All items must be unique in the list.", result.ErrorMessage);
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
    public void UniqueItemsAttribute_DuplicateNumbers_DefaultErrorMessage()
    {
        var model = new { Items = new List<int> { 1, 2, 2 } };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("duplicate items", result.ErrorMessage);
        Assert.Contains("indices 1 and 2", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateNumbers_CustomErrorMessage()
    {
        var model = new { Items = new List<int> { 1, 2, 2 } };
        var attribute = new UniqueItemsAttribute { ErrorMessage = "Numbers must be unique." };
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Numbers must be unique.", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_UniqueComplexObjects_ReturnsSuccess()
    {
        var model = new
        {
            Items = new List<TestItem>
            {
                new TestItem(1, "apple"),
                new TestItem(2, "banana")
            }
        };

        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));
        
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateComplexObjects_DefaultErrorMessage()
    {
        var model = new
        {
            Items = new List<TestItem>
            {
                new TestItem(1, "apple"),
                new TestItem(1, "apple")
            }
        };

        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));
        
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("duplicate items", result.ErrorMessage);
        Assert.Contains("indices 0 and 1", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_DuplicateComplexObjects_CustomErrorMessage()
    {
        var model = new
        {
            Items = new List<TestItem>
            {
                new TestItem(1, "apple"),
                new TestItem(1, "apple")
            }
        };

        var attribute = new UniqueItemsAttribute { ErrorMessage = "Objects in the list must have unique values." };
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));
        
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Objects in the list must have unique values.", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_SingleString_ReturnsError()
    {
        var model = new { Items = "not a list" };
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("must be applied to a collection", result.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_NonCollectionNonString_ReturnsError()
    {
        var model = new { Items = 42 }; // An int, not a string or collection
        var attribute = new UniqueItemsAttribute();
        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("must be applied to a collection", result.ErrorMessage);
    }
}