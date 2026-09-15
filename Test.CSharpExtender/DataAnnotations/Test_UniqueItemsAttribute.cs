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

    // Two unrelated types that share a property name, for mixed-type comparison
    private class Dog(string name)
    {
        public string Name { get; set; } = name;
    }

    private class Cat(string name)
    {
        public string Name { get; set; } = name;
    }

    // Two unrelated types with no properties at all
    private class Empty1;

    private class Empty2;

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

    [Fact]
    public void UniqueItemsAttribute_MixedTypesSharingAPropertyName_ReturnsSuccess()
    {
        var model = new { Items = new List<object> { new Dog("Rex"), new Cat("Rex") } };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_MixedTypesWithNoSharedProperties_ReturnsSuccess()
    {
        var model = new { Items = new List<object> { new Empty1(), new Empty2() } };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_TwoInstancesOfAPropertyLessType_ReturnsError()
    {
        var model = new { Items = new List<object> { new Empty1(), new Empty1() } };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 1", result.ErrorMessage);
    }

    // Overrides Equals and not GetHashCode, so values it calls equal hash
    // differently. A set would put them in separate buckets and never see them.
#pragma warning disable CS0659
    private struct EqualsWithoutHashCode(int id)
    {
        public int Id { get; set; } = id;

        public override bool Equals(object? obj) =>
            obj is EqualsWithoutHashCode other && other.Id == Id;
    }
#pragma warning restore CS0659

    // Overrides both, so it can be trusted
    private readonly struct Money(decimal amount) : IEquatable<Money>
    {
        public decimal Amount { get; } = amount;

        public bool Equals(Money other) => other.Amount == Amount;

        public override bool Equals(object? obj) => obj is Money other && Equals(other);

        public override int GetHashCode() => Amount.GetHashCode();
    }

    // Overrides neither, so ValueType compares and hashes it from the same fields
    private struct PlainPoint(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
    }

    private record struct Reading(int SensorId, double Value);

    // A class whose Equals calls everything equal while its properties differ.
    // The attribute compares classes property by property, so these are unique,
    // and anything judging them by Equals would call them all duplicates.
    private class AlwaysEqual(string name)
    {
        public string Name { get; set; } = name;

        public override bool Equals(object? obj) => true;

        public override int GetHashCode() => 0;
    }

    private static List<string> UniqueStrings(int count) =>
        Enumerable.Range(0, count).Select(i => $"item-{i}").ToList();

    [Fact]
    public void UniqueItemsAttribute_LongListOfUniqueStrings_ReturnsSuccess()
    {
        var model = new { Items = UniqueStrings(100) };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_LongListWithDuplicate_ReportsThePairwiseIndices()
    {
        // Two duplicate pairs, arranged so the pair the pairwise walk finds first
        // (0 and 19) is not the one a set would notice first (1 and 2). The fast
        // path only decides whether to walk, so the reported indices must not move.
        var items = UniqueStrings(20);
        items[2] = items[1];
        items[19] = items[0];

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 19", result!.ErrorMessage);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(100)]
    public void UniqueItemsAttribute_DuplicateStrings_FoundAtEveryListLength(int count)
    {
        // Covers both sides of the length at which the one-pass check switches on
        var items = UniqueStrings(count);
        items[count - 1] = items[0];

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains($"indices 0 and {count - 1}", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_StructOverridingEqualsButNotGetHashCode_StillFindsTheDuplicate()
    {
        // The one-pass check has to refuse to judge this type, or the duplicate
        // goes unseen and invalid data validates
        var items = Enumerable.Range(0, 50).Select(i => new EqualsWithoutHashCode(i)).ToList();
        items[49] = items[0];

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 49", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_StructOverridingBoth_IsJudgedCorrectly()
    {
        var items = Enumerable.Range(0, 50).Select(i => new Money(i)).ToList();
        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        Assert.Equal(ValidationResult.Success,
            attribute.GetValidationResult(model.Items, GetValidationContext(model)));

        items[49] = new Money(0);

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 49", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_StructOverridingNeither_IsJudgedCorrectly()
    {
        var items = Enumerable.Range(0, 50).Select(i => new PlainPoint(i, i * 2)).ToList();
        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        Assert.Equal(ValidationResult.Success,
            attribute.GetValidationResult(model.Items, GetValidationContext(model)));

        items[49] = new PlainPoint(0, 0);

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 49", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_RecordStruct_IsJudgedCorrectly()
    {
        var items = Enumerable.Range(0, 50).Select(i => new Reading(i, i * 1.5)).ToList();
        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        Assert.Equal(ValidationResult.Success,
            attribute.GetValidationResult(model.Items, GetValidationContext(model)));

        items[49] = new Reading(0, 0);

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 49", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_ClassWithValueEquality_IsStillComparedByItsProperties()
    {
        // Every one of these is Equals to every other, so judging them by Equals
        // would call them duplicates. The attribute compares properties, and the
        // names all differ, so they are unique.
        var items = Enumerable.Range(0, 50).Select(i => new AlwaysEqual($"name-{i}")).ToList();

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_LongListContainingNull_IsStillJudgedCorrectly()
    {
        var items = UniqueStrings(30).Cast<string?>().ToList();
        items[10] = null;

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        Assert.Equal(ValidationResult.Success,
            attribute.GetValidationResult(model.Items, GetValidationContext(model)));

        items[20] = null;

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 10 and 20", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_LongListOfComplexObjects_FindsADuplicate()
    {
        var items = Enumerable.Range(0, 50).Select(i => new TestItem(i, $"name-{i}")).ToList();
        items[49] = new TestItem(0, "name-0");

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Contains("indices 0 and 49", result!.ErrorMessage);
    }

    [Fact]
    public void UniqueItemsAttribute_ObjectsDifferingOnlyOnALaterProperty_ReturnsSuccess()
    {
        // Every comparison has to read past the shared Id before it can tell the
        // items apart, which is what the property value cache is for
        var items = Enumerable.Range(0, 40).Select(i => new TestItem(1, $"name-{i}")).ToList();

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_LongListOfGuids_ReturnsSuccess()
    {
        var model = new { Items = Enumerable.Range(0, 50).Select(_ => Guid.NewGuid()).ToList() };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_LongListOfMixedTypes_ReturnsSuccess()
    {
        var items = new List<object>();

        for (int i = 0; i < 20; i++)
        {
            items.Add(i);
            items.Add(i.ToString());
            items.Add(new Dog($"dog-{i}"));
        }

        var model = new { Items = items };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void UniqueItemsAttribute_MixedValueAndReferenceTypes_ReturnsSuccess()
    {
        var model = new { Items = new List<object> { 1, "1", new Dog("1") } };
        var attribute = new UniqueItemsAttribute();

        var result = attribute.GetValidationResult(model.Items, GetValidationContext(model));

        Assert.Equal(ValidationResult.Success, result);
    }
}