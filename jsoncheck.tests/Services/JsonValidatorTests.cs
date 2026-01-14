
using Xunit;
using JsonCheck.Services;

public class JsonValidatorTests
{
    [Fact]
    public void Validate_ValidJsonObject_ReturnsValid()
    {
        var validator = new JsonValidator();
        var result = validator.Validate("{\"a\":1}");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyInput_RetursnEmptyInputError()
    {
        var validator = new JsonValidator();
        var result = validator.Validate("   ");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.EmptyInput, result.ErrorKind);
    }

    [Fact]
    public void Validate_NotJson_ReturnsNotJsonError()
    {
        var validator = new JsonValidator();
        var result = validator.Validate("This it not valid");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.NotJson, result.ErrorKind);
    }

    [Fact]
    public void Validate_SyntadError_ReturnsSyntaxErrorWithLocation()
    {
        var validator = new JsonValidator();
        var result = validator.Validate("{\"a\":}");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotNull(result.LineNumber);
        Assert.NotNull(result.LinePosition);
    }
}