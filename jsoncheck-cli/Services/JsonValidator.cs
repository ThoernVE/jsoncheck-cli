using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonCheck.Services;

public interface IJsonValidator
{
    JsonValidationResult Validate(string json);
}

public sealed class JsonValidator : IJsonValidator
{
    public JsonValidationResult Validate(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new JsonValidationResult
            {
                IsValid = false,
                ErrorKind = JsonErrorKind.EmptyInput,
                ErrorMessage = "Input is empty or whitespace"
            };
        }

        try
        {
            JToken.Parse(json);

            return new JsonValidationResult
            {
                IsValid = true
            };
        }
        catch (JsonReaderException ex)
        {
            return new JsonValidationResult
            {
                IsValid = false,
                ErrorKind = IsLikelyJson(json) ? JsonErrorKind.NotJson : JsonErrorKind.SyntaxError,
                ErrorMessage = ex.Message,
                Path = ex.Path,
                LineNumber = ex.LineNumber,
                LinePosition = ex.LinePosition
            };
        }
        catch (Exception ex)
        {
            return new JsonValidationResult
            {
                IsValid = false,
                ErrorKind = JsonErrorKind.Unknown,
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }

    private static bool IsLikelyJson(string input)
    {
        var trimmed = input.TrimStart();


        return trimmed.StartsWith("{") || trimmed.StartsWith("[");
    }
}

public sealed class JsonValidationResult
{
    public bool IsValid { get; init; }
    public JsonErrorKind? ErrorKind { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Path { get; init; }
    public int? LineNumber { get; init; }
    public int? LinePosition { get; init; }
}