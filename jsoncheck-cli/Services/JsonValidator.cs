using System.Text.Json;

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
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow
            };

            using var _ = JsonDocument.Parse(json, options);

            return new JsonValidationResult
            {
                IsValid = true
            };
        }
        catch (JsonException ex)
        {
            int? line = ex.LineNumber is long ln ? checked((int)ln + 1) : null;
            int? col = ex.BytePositionInLine is long bp ? checked((int)bp + 1) : null;

            return new JsonValidationResult
            {
                IsValid = false,
                ErrorKind = IsLikelyJson(json) ? JsonErrorKind.SyntaxError : JsonErrorKind.NotJson,
                ErrorMessage = ex.Message,
                Path = null,
                LineNumber = line,
                LinePosition = col
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