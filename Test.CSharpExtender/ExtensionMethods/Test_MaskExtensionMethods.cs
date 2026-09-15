using CSharpExtender.DataAnnotations;
using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods;

public class Test_MaskExtensionMethods
{
    private class PaymentDetails
    {
        [Mask(MaskChar = '*', VisiblePrefixLength = 4, VisibleSuffixLength = 4)]
        public string CreditCardNumber { get; set; } = "1234-5678-9012-5678";

        [Mask(VisibleSuffixLength = 4)]
        public string? PhoneNumber { get; set; } = "(555) 123-4567";

        public string CardHolder { get; set; } = "Scott Lilly";
    }

    [Fact]
    public void Mask_CreditCardNumber_KeepsSeparatorsAndBothEnds()
    {
        Assert.Equal("1234-****-****-5678",
            "1234-5678-9012-5678".Mask('*', 4, 4));
    }

    [Fact]
    public void Mask_PhoneNumber_KeepsSeparators()
    {
        Assert.Equal("(***) ***-4567",
            "(555) 123-4567".Mask('*', 0, 4));
    }

    [Fact]
    public void Mask_WithoutPreservingSeparators_MasksEveryCharacter()
    {
        Assert.Equal("1234***********5678",
            "1234-5678-9012-5678".Mask('*', 4, 4, preserveSeparators: false));
    }

    [Fact]
    public void Mask_NoVisibleLengths_MasksEverything()
    {
        Assert.Equal("********", "password".Mask());
    }

    [Fact]
    public void Mask_VisibleLengthsCoverTheValue_MasksEverything()
    {
        // Leaving nothing masked would publish the whole value
        Assert.Equal("****", "1234".Mask('*', 4, 4));
        Assert.Equal("***", "abc".Mask('*', 2, 2));
    }

    [Fact]
    public void Mask_NullOrEmpty_ReturnsInputUnchanged()
    {
        Assert.Null(((string)null).Mask('*', 2, 2));
        Assert.Equal("", "".Mask('*', 2, 2));
    }

    [Fact]
    public void Mask_CustomMaskCharacter_IsUsed()
    {
        Assert.Equal("12##56", "123456".Mask('#', 2, 2));
    }

    [Theory]
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(257)]
    [InlineData(1000)]
    public void Mask_LongStringPastTheStackBuffer_MasksTheMiddle(int length)
    {
        // 256 characters is where the implementation switches from a stack buffer
        // to a heap array, so cover both sides of it
        string value = new string('a', length);

        string result = value.Mask('*', 4, 4);

        Assert.Equal(length, result.Length);
        Assert.Equal("aaaa", result.Substring(0, 4));
        Assert.Equal("aaaa", result.Substring(length - 4));
        Assert.Equal(new string('*', length - 8), result.Substring(4, length - 8));
    }

    private class BaseModel
    {
        [Mask(VisibleSuffixLength = 2)]
        public string Value { get; set; } = "base-value";
    }

    private class DerivedModel : BaseModel
    {
        [Mask(VisibleSuffixLength = 2)]
        public new string Value { get; set; } = "derived-value";
    }

    [Fact]
    public void ToMaskedString_PropertyHiddenByANewOne_DoesNotThrow()
    {
        // Type.GetProperty throws AmbiguousMatchException here. The lookup walks
        // the type's properties and takes the first match instead, which is what
        // the reflection helpers in this library have always done. Which of the
        // two it finds is not guaranteed, because GetProperties does not promise
        // an order, so this pins only that it answers rather than throws.
        string result = new DerivedModel().ToMaskedString(nameof(BaseModel.Value));

        Assert.EndsWith("ue", result);
        Assert.Contains('*', result);
    }

    [Fact]
    public void Mask_NegativeVisibleLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "123456".Mask('*', -1, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => "123456".Mask('*', 2, -1));
    }

    [Fact]
    public void ToMaskedString_AppliesTheAttribute()
    {
        var details = new PaymentDetails();

        Assert.Equal("1234-****-****-5678",
            details.ToMaskedString(nameof(PaymentDetails.CreditCardNumber)));
    }

    [Fact]
    public void ToMaskedString_PropertyWithoutTheAttribute_ReturnsValueUnmasked()
    {
        var details = new PaymentDetails();

        Assert.Equal("Scott Lilly",
            details.ToMaskedString(nameof(PaymentDetails.CardHolder)));
    }

    [Fact]
    public void ToMaskedString_NullValue_ReturnsEmptyString()
    {
        var details = new PaymentDetails { PhoneNumber = null };

        Assert.Equal("", details.ToMaskedString(nameof(PaymentDetails.PhoneNumber)));
    }

    [Fact]
    public void ToMaskedString_UnknownProperty_Throws()
    {
        var details = new PaymentDetails();

        Assert.Throws<ArgumentException>(() => details.ToMaskedString("NoSuchProperty"));
    }

    [Fact]
    public void ToMaskedString_NullObject_Throws()
    {
        object nullObject = null;

        Assert.Throws<ArgumentNullException>(() => nullObject.ToMaskedString("Anything"));
    }

    [Fact]
    public void ToMaskedStrings_ReturnsOnlyMaskedProperties()
    {
        var details = new PaymentDetails();

        var masked = details.ToMaskedStrings();

        Assert.Equal(2, masked.Count);
        Assert.Equal("1234-****-****-5678", masked[nameof(PaymentDetails.CreditCardNumber)]);
        Assert.Equal("(***) ***-4567", masked[nameof(PaymentDetails.PhoneNumber)]);
        Assert.False(masked.ContainsKey(nameof(PaymentDetails.CardHolder)));
    }
}
