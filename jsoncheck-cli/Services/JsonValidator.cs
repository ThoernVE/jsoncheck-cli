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
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }
}

public sealed class JsonValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Path { get; init; }
    public int? LineNumber { get; init; }
    public int? LinePosition { get; init; }
}