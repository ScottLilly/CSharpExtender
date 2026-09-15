using CSharpExtender.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Tests.CSharpExtender.DataAnnotations;

public class Test_ConditionalRequiredAttribute
{
    private static ValidationContext GetValidationContext(object instance) => new(instance);

    private class UserModel
    {
        [ConditionalRequired(DependentProperty = nameof(IsActive), RequiredWhenValue = true)]
        public string? ActiveUserDetails { get; set; }

        public bool IsActive { get; set; }
    }

    private class WhitespaceAllowedModel
    {
        [ConditionalRequired(DependentProperty = nameof(IsActive), RequiredWhenValue = true,
            AllowEmptyStrings = true)]
        public string? ActiveUserDetails { get; set; }

        public bool IsActive { get; set; }
    }

    private class StringConditionModel
    {
        [ConditionalRequired(DependentProperty = nameof(Status), RequiredWhenValue = "Shipped",
            IgnoreCase = true)]
        public string? TrackingNumber { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    private class MissingPropertyModel
    {
        [ConditionalRequired(DependentProperty = "NoSuchProperty", RequiredWhenValue = true)]
        public string? Details { get; set; }
    }

    private class NoDependentPropertyModel
    {
        [ConditionalRequired(RequiredWhenValue = true)]
        public string? Details { get; set; }
    }

    private class CountModel
    {
        [ConditionalRequired(DependentProperty = nameof(Count), RequiredWhenValue = 3)]
        public string? Reason { get; set; }

        public long Count { get; set; }
    }

    [Fact]
    public void ConditionalRequired_ConditionMetAndValueMissing_ReturnsError()
    {
        var model = new UserModel { IsActive = true, ActiveUserDetails = null };

        var results = Validate(model);

        Assert.Single(results);
        Assert.Contains("is required when IsActive is True", results[0].ErrorMessage);
        Assert.Equal(nameof(UserModel.ActiveUserDetails), results[0].MemberNames.Single());
    }

    [Fact]
    public void ConditionalRequired_ConditionMetAndValueSupplied_ReturnsSuccess()
    {
        var model = new UserModel { IsActive = true, ActiveUserDetails = "Details" };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void ConditionalRequired_ConditionNotMetAndValueMissing_ReturnsSuccess()
    {
        var model = new UserModel { IsActive = false, ActiveUserDetails = null };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void ConditionalRequired_ConditionMetAndValueIsWhitespace_ReturnsError()
    {
        var model = new UserModel { IsActive = true, ActiveUserDetails = "   " };

        var results = Validate(model);

        Assert.Single(results);
    }

    [Fact]
    public void ConditionalRequired_WhitespaceAllowed_ReturnsSuccess()
    {
        var model = new WhitespaceAllowedModel { IsActive = true, ActiveUserDetails = "   " };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void ConditionalRequired_StringConditionMatchesIgnoringCase_ReturnsError()
    {
        var model = new StringConditionModel { Status = "shipped", TrackingNumber = null };

        var results = Validate(model);

        Assert.Single(results);
    }

    [Fact]
    public void ConditionalRequired_StringConditionDoesNotMatch_ReturnsSuccess()
    {
        var model = new StringConditionModel { Status = "Pending", TrackingNumber = null };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void ConditionalRequired_IntegerLiteralAgainstALongProperty_ReturnsError()
    {
        // RequiredWhenValue is an int, Count is a long, so a plain Equals would
        // never see the condition as met
        var model = new CountModel { Count = 3, Reason = null };

        var results = Validate(model);

        Assert.Single(results);
    }

    [Fact]
    public void ConditionalRequired_UnknownDependentProperty_ReturnsError()
    {
        var model = new MissingPropertyModel { Details = null };

        var results = Validate(model);

        Assert.Single(results);
        Assert.Contains("NoSuchProperty was not found", results[0].ErrorMessage);
    }

    [Fact]
    public void ConditionalRequired_NoDependentPropertyName_ReturnsError()
    {
        var model = new NoDependentPropertyModel { Details = null };

        var results = Validate(model);

        Assert.Single(results);
        Assert.Contains("must be given a DependentProperty name", results[0].ErrorMessage);
    }

    [Fact]
    public void ConditionalRequired_CustomErrorMessage_IsUsed()
    {
        var attribute = new ConditionalRequiredAttribute
        {
            DependentProperty = nameof(UserModel.IsActive),
            RequiredWhenValue = true,
            ErrorMessage = "Details are needed for an active user."
        };
        var model = new UserModel { IsActive = true };

        var result = attribute.GetValidationResult(model.ActiveUserDetails,
            GetValidationContext(model));

        Assert.Equal("Details are needed for an active user.", result!.ErrorMessage);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, GetValidationContext(model), results, true);

        return results;
    }
}
