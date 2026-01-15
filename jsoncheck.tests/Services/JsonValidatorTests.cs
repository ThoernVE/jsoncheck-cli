
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
    public void Validate_EmptyInput_ReturnsEmptyInputError()
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
    public void Validate_SyntaxError_ReturnsSyntaxErrorWithLocation()
    {
        var validator = new JsonValidator();
        var result = validator.Validate("{\"a\":}");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotNull(result.LineNumber);
        Assert.NotNull(result.LinePosition);
    }

    [Fact]
    public void Validate_UnquotedPropertyAndTrailingComma_IsInvalid()
    {
        var validator = new JsonValidator();
        var input = "{ Nummer: 1234, }";

        var result = validator.Validate(input);

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
    }

    [Fact]
    public void Validate_TrailingCommaInArray_IsInvalid()
    {
        var validator = new JsonValidator();
        var input = "[1, 2,]";

        var result = validator.Validate(input);

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
    }

    [Fact]
    public void Validate_JsonWithComments_IsInvalid()
    {
        var validator = new JsonValidator();
        var input = "{ \"a\": 1 /* comment */ }";

        var result = validator.Validate(input);

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
    }

    [Fact]
    public void Validate_ValidJsonArray_ReturnsValid()
    {
        var validator = new JsonValidator();
        var result = validator.Validate("[1,2,3]");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhitespaceAroundJson_ReturnsValid()
    {
        var validator = new JsonValidator();
        var result = validator.Validate(" \n\t { \"a\": 1 } \r\n ");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InputStartingWithBrace_IsSyntaxError()
    {
        var validator = new JsonValidator();

        var result = validator.Validate("{ this is not valid json");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
    }

    [Fact]
    public void Validate_InputStartingWithBracket_IsSyntaxError()
    {
        var validator = new JsonValidator();

        var result = validator.Validate("[ not json");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
    }

    [Fact]
    public void Validate_PlainText_IsNotJson()
    {
        var validator = new JsonValidator();

        var result = validator.Validate("hello world");

        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.NotJson, result.ErrorKind);
    }

    [Fact]
    public void Validate_WhitespaceBeforeBrace_IsSyntaxError()
    {
        var validator = new JsonValidator();
    
        var result = validator.Validate("   \n\t { bad json }");
    
        Assert.False(result.IsValid);
        Assert.Equal(JsonErrorKind.SyntaxError, result.ErrorKind);
    }
}